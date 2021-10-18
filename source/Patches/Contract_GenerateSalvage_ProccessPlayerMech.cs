using System;
using System.Linq;
using BattleTech;
using CustomComponents;
using Harmony;
using UIWidgetsSamples;
using UnityEngine;
using Contract_GenerateSalvage = CustomSalvage.Contract_GenerateSalvage;
using ContractHelper = CustomSalvage.ContractHelper;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(Contract_GenerateSalvage))]
    [HarmonyPatch("ProccessPlayerMech")]
    public static class Contract_GenerateSalvage_ProccessPlayerMech
    {
        [HarmonyPrefix]
        public static bool ProcessPlayerVehicle(UnitResult unitResult, ContractHelper Contract)
        {
            var mech = unitResult.mech;
            if (Control.Instance.Settings.LostVehicleAction == PlayerVehicleAction.None)
            {
                Control.Instance.LogDebug(DInfo.Salvage, $"- None action for player vehicle, skipping");
                return true;
            }

            var original = UnityGameInstance.BattleTechGame.DataManager.MechDefs.Get(mech.Description.Id);

            if (!original.IsVehicle())
            {
                Control.Instance.LogDebug(DInfo.Salvage, $"- {mech.Description.Id} with GUID {mech.GUID} is not vehicle, return to mech process");
                return true;
            }


            //Control.Instance.LogDebug(DInfo.Salvage, "Found vehicle {0}, guid:[{1}]", mech.Description.Id, mech.GUID);
            //Control.Instance.LogDebug(DInfo.Salvage, "-- all units:");
            //foreach (var actor in Contract.Contract.BattleTechGame.Combat.AllActors)
            //{
            //    string unit_guid = "";
            //    if (actor is Vehicle v)
            //        unit_guid = v.VehicleDef.GUID;
            //    else if (actor is Mech m)
            //        unit_guid = m.MechDef.GUID;


            //    Control.Instance.LogDebug(DInfo.Salvage, "{0}/{5}({3}/{6}): {2}[{1}] - {4}", actor.GUID, actor.Description.Id, actor.DisplayName, actor.UnitType, actor.DeathMethod, unit_guid, actor.Type.ToString());
            //}

            Control.Instance.LogDebug(DInfo.Salvage, "-- vehicles:");
            Vehicle vehicle = null;
            foreach (var v in Contract.Contract.BattleTechGame.Combat.AllActors.OfType<Vehicle>())
            {
                //Control.Instance.LogDebug(DInfo.Salvage, "{0}/{5}({3}/{6}): {2}[{1}] - {4}", v.GUID, v.Description.Id, v.DisplayName, v.UnitType, v.DeathMethod, v.VehicleDef.GUID, v.Type.ToString());
                if (v.VehicleDef.GUID == mech.GUID)
                {
                    vehicle = v;
                    break;
                }
            }

            if (vehicle == null)
            {
                Control.Instance.LogError($"Vehicle {mech.Description.Id} with GUID {mech.GUID} not found, return vehicle to player fallback");
                unitResult.mechLost = false;
                return false;
            }

            unitResult.mechLost = !CanRecover(vehicle);
            if (unitResult.mechLost)
            {
                switch (Control.Instance.Settings.LostVehicleAction)
                {
                    case PlayerVehicleAction.Salvage:
                        SalvageVehicle(Contract, vehicle, mech);
                        break;
                    case PlayerVehicleAction.Return:
                        ReturnVehicle(Contract, vehicle, mech);
                        break;
                    case PlayerVehicleAction.SalvageParts:
                        SalvageParts(Contract, vehicle, mech);
                        break;
                    case PlayerVehicleAction.ReturnParts:
                        ReturnParts(Contract, vehicle, mech);
                        break;
                }
            }

            return false;
        }

       private static bool IsDead(this Vehicle vehicle)
        {
            Control.Instance.LogDebug(DInfo.Death, "Check Death for {0}, deathmethod: {1}", vehicle.VehicleDef.Description.Id, vehicle.DeathMethod);

            //if (vehicle.VehicleDef.Chassis.HasTurret)
            //    if (vehicle.GetLocationDamageLevel(VehicleChassisLocations.Turret) == LocationDamageLevel.Destroyed)
            //    {
            //        Control.Instance.LogDebug(DInfo.Death, "- Turret destroyed");
            //        return true;
            //    }

            //if (vehicle.GetLocationDamageLevel(VehicleChassisLocations.Front) == LocationDamageLevel.Destroyed)
            //{
            //    Control.Instance.LogDebug(DInfo.Death, "- Front destroyed");
            //    return true;
            //}
            //if (vehicle.GetLocationDamageLevel(VehicleChassisLocations.Rear) == LocationDamageLevel.Destroyed)
            //{
            //    Control.Instance.LogDebug(DInfo.Death, "- Rear destroyed");
            //    return true;
            //}
            //if (vehicle.GetLocationDamageLevel(VehicleChassisLocations.Left) == LocationDamageLevel.Destroyed)
            //{
            //    Control.Instance.LogDebug(DInfo.Death, "- Left destroyed");
            //    return true;
            //}
            //if (vehicle.GetLocationDamageLevel(VehicleChassisLocations.Right) == LocationDamageLevel.Destroyed)
            //{
            //    Control.Instance.LogDebug(DInfo.Death, "- Right destroyed");
            //    return true;
            //}
            //Control.Instance.LogDebug(DInfo.Death, "- Not dead");
            var dm = vehicle.DeathMethod;

            return dm != DeathMethod.NOT_SET && dm != DeathMethod.DespawnedEscaped &&
                   dm != DeathMethod.DespawnedNoMessage;
        }

        private static bool CanRecover(Vehicle vehicle)
        {
            if (!vehicle.IsDead())
                return true;

            switch (Control.Instance.Settings.Recovery)
            {
                case PlayerVehicleRecoveryType.AlwaysRecovery:
                    Control.Instance.LogDebug(DInfo.Salvage, " --- no recovery");

                    return true;
                case PlayerVehicleRecoveryType.NoRecovery:
                    Control.Instance.LogDebug(DInfo.Salvage, " --- allways recovery");
                    return false;

                case PlayerVehicleRecoveryType.SimGameConstant:
                    var chance = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance + Control.Instance.Settings.RecoveryChanceConstantMod;
                    var rnd = CustomShops.Control.State.Sim.NetworkRandom.Float();
                    Control.Instance.LogDebug(DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, {2}", chance, rnd, (rnd < chance ? "recovered" : "failed"));
                    return rnd < chance;

                case PlayerVehicleRecoveryType.HpLeft:
                    var total = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP+ vehicle.SummaryStructureMax;
                    var current = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;
                    var max = (current / total) * Control.Instance.Settings.RecoveryChanceHPMod + Control.Instance.Settings.RecoveryChanceHPBase ;
                    var roll = CustomShops.Control.State.Sim.NetworkRandom.Float();
                    Control.Instance.LogDebug(DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, {2}", max, roll, (roll < max ? "recovered" : "failed"));
                    return roll < max;
                case PlayerVehicleRecoveryType.HpLeftConstant:
                    var totalhp = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureMax;
                    var currenthp = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;
                    var maxhp = (currenthp / totalhp + Control.Instance.Settings.RecoveryChanceHPBase) * Control.Instance.Settings.RecoveryChanceHPMod;
                    var bchance = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance +
                                  Control.Instance.Settings.RecoveryChanceConstantMod;
                    var tchance = bchance + maxhp;

                    var r = CustomShops.Control.State.Sim.NetworkRandom.Float();
                    Control.Instance.LogDebug(DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, base:{2:0.00}, Hp:{3:0.00} {4}", tchance, r, bchance, maxhp, (r < tchance ? "recovered" : "failed"));
                    return r < tchance;
            }
            return false;
        }


        private static int NumParts(Vehicle vehicle, SimGameState simgame)
        {
            int min_parts = 1;
            int max_parts = simgame.Constants.Story.DefaultMechPartMax;

            var total = vehicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureMax;
            var current = vehicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vehicle.SummaryStructureCurrent;

            var parts = Mathf.Clamp(Mathf.CeilToInt(current / total * max_parts), min_parts, max_parts);
            Control.Instance.LogDebug(DInfo.Salvage, "-- hp: {0:0.0}/{1:0.0} parts:{2}",
                current, total, parts);
            return parts;
        }

        public static void ReturnVehicle(ContractHelper contract, Vehicle vehicle, MechDef mech)
        {
            var vid = vehicle.VehicleDef.Description.Id;
            var vdef = vehicle.VehicleDef;

            if (vdef == null)
            {
                Control.Instance.LogError("No vehicledef for return");
            }

            ReturnParts(contract, vehicle, mech);
            var chance = Control.Instance.Settings.ModuleRecoveryChance;
            foreach (var component in vehicle.VehicleDef.Inventory)
            {
                var rnd = CustomShops.Control.State.Sim.NetworkRandom.Float();

                if (component.DamageLevel != ComponentDamageLevel.Destroyed)
                {
                    if (rnd < chance)
                    {

                        Control.Instance.LogDebug(DInfo.Salvage, "-- {1:0.00}<{2:0.00}, recovered {0}", component.ComponentDefID, rnd, chance);
                        contract.AddComponentToFinalSalvage(component.Def);
                    }
                    else
                    {
                        Control.Instance.LogDebug(DInfo.Salvage, "-- {1:0.00}>{2:0.00}, destroyed {0}", component.ComponentDefID, rnd, chance);
                    }

                }
                else
                {
                    Control.Instance.LogDebug(DInfo.Salvage, "-- {0}, DESTROYED", component.ComponentDefID);
                }
            }
        }


        public static void SalvageVehicle(ContractHelper contract, Vehicle vehicle, MechDef mech)
        {
            var vid = vehicle.VehicleDef.Description.Id;
            var vdef = vehicle.VehicleDef;

            if (vdef == null)
            {
                Control.Instance.LogError("No vehicledef for return");
            }

            SalvageParts(contract, vehicle, mech);

            var chance = Control.Instance.Settings.ModuleRecoveryChance;

            foreach (var component in vehicle.VehicleDef.Inventory)
            {
                var rnd = CustomShops.Control.State.Sim.NetworkRandom.Float();

                if (component.DamageLevel != ComponentDamageLevel.Destroyed)
                {
                    if (rnd < chance)
                    {

                        Control.Instance.LogDebug(DInfo.Salvage, "-- {1:0.00}<{2:0.00}, recovered {0}", component.ComponentDefID, rnd, chance);
                        contract.AddComponentToPotentialSalvage(component.Def, component.DamageLevel, false);
                    }
                    else
                    {
                        Control.Instance.LogDebug(DInfo.Salvage, "-- {1:0.00}>{2:0.00}, destroyed {0}", component.ComponentDefID, rnd, chance);
                    }

                }
                else
                {
                    Control.Instance.LogDebug(DInfo.Salvage, "-- {0}, DESTROYED", component.ComponentDefID);
                }
            }
        }

        private static void ReturnParts(ContractHelper contract, Vehicle vehicle, MechDef mech)
        {
            if(!string.IsNullOrEmpty(Control.Instance.Settings.NoVehiclePartsTag))
                if (vehicle.VehicleDef.VehicleTags.Contains(Control.Instance.Settings.NoVehiclePartsTag))
                {
                    Control.Instance.LogDebug(DInfo.Salvage, "Returning {0} - no parts by tags", mech.Description.Id);
                    return;
                }

            var simgame = contract.Contract.BattleTechGame.Simulation;
            var parts = NumParts(vehicle, simgame);

            if (string.IsNullOrEmpty(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag) || !vehicle.VehicleDef.VehicleTags.Contains(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag))
                contract.AddMechPartsToFinalSalvage(simgame.Constants, mech, parts);
        }

        private static void SalvageParts(ContractHelper contract, Vehicle vehicle, MechDef mech)
        {
            if (!string.IsNullOrEmpty(Control.Instance.Settings.NoVehiclePartsTag))
                if (vehicle.VehicleDef.VehicleTags.Contains(Control.Instance.Settings.NoVehiclePartsTag))
                {
                    Control.Instance.LogDebug(DInfo.Salvage, "Salvaging {0} - no parts by tags", mech.Description.Id);
                    return;
                }

            var simgame = contract.Contract.BattleTechGame.Simulation;
            var parts = NumParts(vehicle, simgame);

            if (string.IsNullOrEmpty(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag) || !vehicle.VehicleDef.VehicleTags.Contains(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag))
                contract.AddMechPartsToPotentialSalvage(simgame.Constants, mech, parts);
        }
    }


}
