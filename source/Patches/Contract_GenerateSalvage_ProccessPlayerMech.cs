using System.Linq;
using BattleTech;
using UnityEngine;
using Contract_GenerateSalvage = CustomSalvage.Contract_GenerateSalvage;
using ContractHelper = CustomSalvage.ContractHelper;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(Contract_GenerateSalvage))]
[HarmonyPatch("ProccessPlayerMech")]
public static class Contract_GenerateSalvage_ProccessPlayerMech
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, UnitResult unitResult, ContractHelper Contract)
    {
        var mech = unitResult.mech;
        if (Control.Instance.Settings.LostVehicleAction == PlayerVehicleAction.None)
        {
            Log.Main.Debug?.Log($"- None action for player vehicle, skipping");
            return;
        }

        var original = UnityGameInstance.BattleTechGame.DataManager.MechDefs.Get(mech.Description.Id);

        if (!original.IsVehicle())
        {
            Log.Main.Debug?.Log($"- {mech.Description.Id} with GUID {mech.GUID} is not vehicle, return to mech process");
            return;
        }

        Log.Main.Debug?.Log("-- vehicles:");
        Mech vehicle = null;
        foreach (var v in Contract.Contract.BattleTechGame.Combat.AllActors.OfType<Mech>())
        {
            if (v.PilotableActorDef.GUID == mech.GUID)
            {
                vehicle = v;
                break;
            }
        }

        if (vehicle == null)
        {
            Log.Main.Error?.Log($"Vehicle {mech.Description.Id} with GUID {mech.GUID} not found, return vehicle to player fallback");
            unitResult.mechLost = false;
            __runOriginal = false;
            return;
        }

        unitResult.mechLost = !CanRecoverVehicle(vehicle);
        if (unitResult.mechLost)
        {
            switch (Control.Instance.Settings.LostVehicleAction)
            {
                case PlayerVehicleAction.Salvage:
                    SalvageVehicle(Contract, vehicle, mech, false);
                    break;
                case PlayerVehicleAction.Return:
                    SalvageVehicle(Contract, vehicle, mech, true);
                    break;
                case PlayerVehicleAction.SalvageParts:
                    SalvageParts(Contract, vehicle, mech, false);
                    break;
                case PlayerVehicleAction.ReturnParts:
                    SalvageParts(Contract, vehicle, mech, true);
                    break;
            }
        }

        __runOriginal = false;
    }

    private static bool IsDeadVehicle(this AbstractActor vehicle)
    {
        Log.Main.Debug?.Log($"Check Death for {vehicle.Description.Id}, deathmethod: {vehicle.DeathMethod}");

        return vehicle.DeathMethod switch
        {
            DeathMethod.NOT_SET => false,
            DeathMethod.DespawnedEscaped => false,
            DeathMethod.DespawnedNoMessage => false,
            _ => true
        };
    }

    private static bool CanRecoverVehicle(AbstractActor vehicle)
    {
        if (!vehicle.IsDeadVehicle())
            return true;

        switch (Control.Instance.Settings.Recovery)
        {
            case PlayerVehicleRecoveryType.AlwaysRecovery:
                Log.Main.Debug?.Log(" --- allways recovery");

                return true;
            case PlayerVehicleRecoveryType.NoRecovery:
                Log.Main.Debug?.Log(" --- no recovery");
                return false;

            case PlayerVehicleRecoveryType.SimGameConstant:
                var chance = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance + Control.Instance.Settings.RecoveryChanceConstantMod;
                var rnd = CustomShops.Control.State.Sim.NetworkRandom.Float();
                Log.Main.Debug?.Log(
                    $" --- chance:{chance:0.00} roll:{rnd:0.00}, {(rnd < chance ? "recovered" : "failed")}");
                return rnd < chance;

            case PlayerVehicleRecoveryType.HpLeft:
                var total = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP+ vehicle.SummaryStructureMax;
                var current = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;
                var max = (current / total) * Control.Instance.Settings.RecoveryChanceHPMod + Control.Instance.Settings.RecoveryChanceHPBase ;
                var roll = CustomShops.Control.State.Sim.NetworkRandom.Float();
                Log.Main.Debug?.Log(
                    $" --- chance:{max:0.00} roll:{roll:0.00}, {(roll < max ? "recovered" : "failed")}");
                return roll < max;
            case PlayerVehicleRecoveryType.HpLeftConstant:
                var totalhp = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureMax;
                var currenthp = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;
                var maxhp = (currenthp / totalhp + Control.Instance.Settings.RecoveryChanceHPBase) * Control.Instance.Settings.RecoveryChanceHPMod;
                var bchance = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance +
                              Control.Instance.Settings.RecoveryChanceConstantMod;
                var tchance = bchance + maxhp;

                var r = CustomShops.Control.State.Sim.NetworkRandom.Float();
                Log.Main.Debug?.Log(
                    $" --- chance:{tchance:0.00} roll:{r:0.00}, base:{bchance:0.00}, Hp:{maxhp:0.00} {(r < tchance ? "recovered" : "failed")}");
                return r < tchance;
        }
        return false;
    }


    private static int NumParts(Mech vehicle, SimGameState simgame)
    {
        int min_parts = 1;
        int max_parts = simgame.Constants.Story.DefaultMechPartMax;

        var total = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureMax;
        var current = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;

        var parts = Mathf.Clamp(Mathf.CeilToInt(current / total * max_parts), min_parts, max_parts);
        Log.Main.Debug?.Log($"-- hp: {current:0.0}/{total:0.0} parts:{parts}");
        return parts;
    }

    private static void SalvageVehicle(ContractHelper contract, Mech vehicle, MechDef mech, bool isFinal)
    {
        var vdef = vehicle.MechDef;
        if (vdef == null)
        {
            Log.Main.Error?.Log("No vehicledef for return");
        }

        SalvageParts(contract, vehicle, mech, isFinal);

        var chance = Control.Instance.Settings.ModuleRecoveryChance;
        foreach (var component in vehicle.MechDef.Inventory)
        {
            var rnd = CustomShops.Control.State.Sim.NetworkRandom.Float();

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

    private static void SalvageParts(ContractHelper contract, Mech vehicle, MechDef mech, bool isFinal)
    {
        if (vehicle.MechDef.IsNoVehicleParts() || vehicle.MechDef.IsNoSalvage())
        {
            Log.Main.Debug?.Log($"Returning {mech.Description.Id} - no parts by tags");
            return;
        }

        var simgame = contract.Contract.BattleTechGame.Simulation;
        Log.Main.Debug?.Log($"Salvaging {mech.Description.Id} parts isFinal={isFinal}");
        var parts = NumParts(vehicle, simgame);

        if (isFinal)
        {
            contract.AddMechPartsToFinalSalvage(simgame.Constants, mech, parts);
        }
        else
        {
            contract.AddMechPartsToPotentialSalvage(simgame.Constants, mech, parts);
        }
    }
}