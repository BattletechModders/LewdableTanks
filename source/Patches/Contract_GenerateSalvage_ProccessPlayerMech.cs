using System.Linq;
using BattleTech;
using UnityEngine;
using Contract_GenerateSalvage = CustomSalvage.Contract_GenerateSalvage;
using ContractHelper = CustomSalvage.ContractHelper;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(Contract_GenerateSalvage))]
[HarmonyPatch("ProccessPlayerMech")]
internal static class Contract_GenerateSalvage_ProccessPlayerMech
{
    private static Settings SSettings => Control.Instance.Settings;
    private static GameInstance SBattleTechGame => UnityGameInstance.BattleTechGame;
    private static SimGameConstants SSimGameConstants => SBattleTechGame.Simulation.Constants;
    private static NetworkRandom SNetworkRandom => SBattleTechGame.Simulation.NetworkRandom;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, UnitResult unitResult, ContractHelper Contract)
    {
        var mechDef = unitResult.mech;
        if (SSettings.LostVehicleAction == PlayerVehicleAction.None)
        {
            Log.Main.Debug?.Log($"- None action for player vehicle, skipping");
            return;
        }

        var original = SBattleTechGame.DataManager.MechDefs.Get(mechDef.Description.Id);

        if (!original.IsVehicle())
        {
            Log.Main.Debug?.Log($"- {mechDef.Description.Id} with GUID {mechDef.GUID} is not vehicle, return to mech process");
            return;
        }

        Log.Main.Debug?.Log("-- vehicles:");
        var mech = SBattleTechGame.Combat.AllActors.OfType<Mech>()
            .FirstOrDefault(v => v.PilotableActorDef.GUID == mechDef.GUID);

        if (mech == null)
        {
            Log.Main.Error?.Log($"Vehicle {mechDef.Description.Id} with GUID {mechDef.GUID} not found, return vehicle to player fallback");
            unitResult.mechLost = false;
            __runOriginal = false;
            return;
        }

        unitResult.mechLost = !CanRecoverVehicle(mech);
        if (unitResult.mechLost)
        {
            switch (SSettings.LostVehicleAction)
            {
                case PlayerVehicleAction.Salvage:
                    SalvageVehicle(Contract, mech, mechDef, false);
                    break;
                case PlayerVehicleAction.Return:
                    SalvageVehicle(Contract, mech, mechDef, true);
                    break;
                case PlayerVehicleAction.SalvageParts:
                    SalvageParts(Contract, mech, mechDef, false);
                    break;
                case PlayerVehicleAction.ReturnParts:
                    SalvageParts(Contract, mech, mechDef, true);
                    break;
            }
        }

        __runOriginal = false;
    }

    private static bool IsDeadVehicle(this AbstractActor actor)
    {
        Log.Main.Debug?.Log($"Check Death for {actor.Description.Id}, deathmethod: {actor.DeathMethod}");

        return actor.DeathMethod switch
        {
            DeathMethod.NOT_SET => false,
            DeathMethod.DespawnedEscaped => false,
            DeathMethod.DespawnedNoMessage => false,
            _ => true
        };
    }

    private static bool CanRecoverVehicle(AbstractActor actor)
    {
        if (!actor.IsDeadVehicle())
            return true;

        switch (SSettings.Recovery)
        {
            case PlayerVehicleRecoveryType.AlwaysRecovery:
                Log.Main.Debug?.Log(" --- allways recovery");

                return true;
            case PlayerVehicleRecoveryType.NoRecovery:
                Log.Main.Debug?.Log(" --- no recovery");
                return false;

            case PlayerVehicleRecoveryType.SimGameConstant:
                var chance = SSimGameConstants.Salvage.DestroyedMechRecoveryChance + SSettings.RecoveryChanceConstantMod;
                var rnd = SNetworkRandom.Float();
                Log.Main.Debug?.Log($" --- chance:{chance:0.00} roll:{rnd:0.00}, {(rnd < chance ? "recovered" : "failed")}");
                return rnd < chance;

            case PlayerVehicleRecoveryType.HpLeft:
                var total = actor.SummaryArmorMax * SSettings.ArmorEffectOnHP+ actor.SummaryStructureMax;
                var current = actor.SummaryArmorCurrent * SSettings.ArmorEffectOnHP + actor.SummaryStructureCurrent;
                var max = current / total * SSettings.RecoveryChanceHPMod + SSettings.RecoveryChanceHPBase ;
                var roll = SNetworkRandom.Float();
                Log.Main.Debug?.Log($" --- chance:{max:0.00} roll:{roll:0.00}, {(roll < max ? "recovered" : "failed")}");
                return roll < max;
            case PlayerVehicleRecoveryType.HpLeftConstant:
                var totalhp = actor.SummaryArmorMax * SSettings.ArmorEffectOnHP + actor.SummaryStructureMax;
                var currenthp = actor.SummaryArmorCurrent * SSettings.ArmorEffectOnHP + actor.SummaryStructureCurrent;
                var maxhp = (currenthp / totalhp + SSettings.RecoveryChanceHPBase) * SSettings.RecoveryChanceHPMod;
                var bchance = SSimGameConstants.Salvage.DestroyedMechRecoveryChance +
                              SSettings.RecoveryChanceConstantMod;
                var tchance = bchance + maxhp;

                var r = SNetworkRandom.Float();
                Log.Main.Debug?.Log($" --- chance:{tchance:0.00} roll:{r:0.00}, base:{bchance:0.00}, Hp:{maxhp:0.00} {(r < tchance ? "recovered" : "failed")}");
                return r < tchance;
        }
        return false;
    }

    private static int NumParts(Mech mech)
    {
        const int minParts = 1;
        var maxParts = SSimGameConstants.Story.DefaultMechPartMax;

        var total = mech.SummaryArmorMax * SSettings.ArmorEffectOnHP + mech.SummaryStructureMax;
        var current = mech.SummaryArmorCurrent * SSettings.ArmorEffectOnHP + mech.SummaryStructureCurrent;

        var parts = Mathf.Clamp(Mathf.CeilToInt(current / total * maxParts), minParts, maxParts);
        Log.Main.Debug?.Log($"-- hp: {current:0.0}/{total:0.0} parts:{parts}");
        return parts;
    }

    private static void SalvageVehicle(ContractHelper contract, Mech mech, MechDef mechDef, bool isFinal)
    {
        SalvageParts(contract, mech, mechDef, isFinal);

        if (mech.MechDef == null)
        {
            Log.Main.Error?.Log("-- MechDef is null for vehicle, ignoring component salvage");
            return;
        }

        var chance = SSettings.ModuleRecoveryChance;
        foreach (var component in mech.MechDef.Inventory)
        {
            var rnd = SNetworkRandom.Float();

            if (component.DamageLevel != ComponentDamageLevel.Destroyed)
            {
                if (rnd < chance)
                {
                    Log.Main.Debug?.Log($"-- {rnd:0.00}<{chance:0.00}, recovered {component.ComponentDefID} isFinal={isFinal}");
                    if (isFinal)
                    {
                        contract.AddComponentToFinalSalvage(component.Def);
                    }
                    else
                    {
                        contract.AddComponentToPotentialSalvage(component.Def, component.DamageLevel, false);
                    }
                }
                else
                {
                    Log.Main.Debug?.Log($"-- {rnd:0.00}>{chance:0.00}, destroyed {component.ComponentDefID}");
                }

            }
            else
            {
                Log.Main.Debug?.Log($"-- {component.ComponentDefID}, DESTROYED");
            }
        }
    }

    private static void SalvageParts(ContractHelper contract, Mech mech, MechDef mechDef, bool isFinal)
    {
        if (mechDef.IsNoVehicleParts() || mechDef.IsNoSalvage())
        {
            Log.Main.Debug?.Log($"Returning {mechDef.Description.Id} - no parts by tags");
            return;
        }

        Log.Main.Debug?.Log($"Salvaging {mechDef.Description.Id} parts isFinal={isFinal}");
        var parts = NumParts(mech);

        if (isFinal)
        {
            contract.AddMechPartsToFinalSalvage(SSimGameConstants, mechDef, parts);
        }
        else
        {
            contract.AddMechPartsToPotentialSalvage(SSimGameConstants, mechDef, parts);
        }
    }
}