using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleTech;
using CustomComponents;
using Harmony;

namespace LewdableTanks
{
    public static class Extentions
    {
        public static bool IsVehicle(this MechDef mech)
        {
            if (mech == null)
            {
                return false;
            }

            if (mech.MechTags != null && mech.MechTags.Contains("fake_vehicle"))
            {
                return true;
            }

            return IsVehicle(mech.Chassis);
        }

        public static bool IsVehicle(this ChassisDef chassis)
        {
            if (chassis == null)
            {
                return false;
            }

            return chassis.ChassisTags != null && chassis.ChassisTags.Contains("fake_vehicle_chassis");
        }

        public static void FixVehicleCost(List<MechDef> mechdefs, SimGameState simgame)
        {
            var k = Control.Instance.Settings.VehiclePartCostMult;

            foreach (var mechdef in mechdefs)
            {
                if (!IsVehicle(mechdef))
                {
                    continue;
                }
                var t = new Traverse(mechdef);
                t.Field<int>("simGameMechPartCost").Value = (int)(mechdef.Description.Cost / 5 * k);
                Control.Instance.LogDebug(DInfo.AutoFix, "Fixing cost of {0} set to {1}", mechdef.Description.Id,
                    mechdef.SimGameMechPartCost);
            }
        }

        public static void FixDescription(List<MechDef> mechdefs, SimGameState simgame)
        {
            foreach (var mechdef in mechdefs)
            {
                if (!IsVehicle(mechdef))
                {
                    continue;
                }

                if (!mechdef.Description.Details.Contains("#Weapons:"))
                {
                    var sb = new StringBuilder("\n#Weapons:\n");
                    var d = new Dictionary<string, int>();
                    foreach (var item in mechdef.Inventory
                        .Where(i => i.ComponentDefType == ComponentType.Weapon)
                        .Select(i => i.Def as WeaponDef))
                    {
                        if (d.ContainsKey(item.Description.UIName))
                            d[item.Description.UIName] += 1;
                        else
                            d[item.Description.UIName] = 1;
                    }

                    foreach (var pair in d)
                    {
                        sb.Append($"  {pair.Value}x {pair.Key}\n");
                    }

                    var str = sb.ToString();

                    var tm = new Traverse(mechdef.Description);
                    var tc = new Traverse(mechdef.Chassis.Description);

                    tm.Property<string>("Details").Value = mechdef.Description.Details + str;
                    tc.Property<string>("Details").Value = mechdef.Chassis.Description.Details + str;
                }
            }
        }

        public static void FixVehicleUIName(List<MechDef> mechdefs, SimGameState simgame)
        {
            if (UnityGameInstance.BattleTechGame.Simulation == null ||
                UnityGameInstance.BattleTechGame.Simulation.DataManager == null)
                return;

            foreach (var mechdef in mechdefs)
            {
                if (!string.IsNullOrEmpty(mechdef.Description.UIName))
                    continue;

                if (!IsVehicle(mechdef))
                {
                    continue;
                }

                var tm = new Traverse(mechdef.Description);
                var tc = new Traverse(mechdef.Chassis.Description);

                var v = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(mechdef.Description.Id);

                var name = (v.Chassis.Is<UIName>(out var uiName) && !string.IsNullOrEmpty(uiName.N))
                    ? uiName.N
                    : v.Description.Name;

                tm.Property<string>("UIName").Value = name;
                tc.Property<string>("UIName").Value = name;

            }
        }
    }
}