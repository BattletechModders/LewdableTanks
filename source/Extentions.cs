using BattleTech;
using CustomComponents;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LewdableTanks
{
  public static class Extentions
  {
    public static bool IsVehicle(this MechDef mech) => mech != null && mech.MechTags != null && mech.Chassis.ChassisTags.Contains(Control.Instance.Settings.FakeVehicleTag);

    public static bool IsVehicle(this ChassisDef chassis) => chassis != null && chassis.ChassisTags != null && chassis.ChassisTags.Contains(Control.Instance.Settings.FakeVehicleTag);

    public static void FixVehicleCost(List<MechDef> mechdefs, SimGameState simgame)
    {
      float vehiclePartCostMult = Control.Instance.Settings.VehiclePartCostMult;
      foreach (MechDef mechdef in mechdefs)
      {
        new Traverse((object) mechdef).Field<int>("simGameMechPartCost").Value = (int) ((double) (mechdef.Description.Cost / 5) * (double) vehiclePartCostMult);
        Control.Instance.LogDebug(DInfo.AutoFix, "Fixing cost of {0} set to {1}", (object) mechdef.Description.Id, (object) mechdef.SimGameMechPartCost);
      }
    }

    public static void FixDescription(List<MechDef> mechdefs, SimGameState simgame)
    {
      foreach (MechDef mechdef in mechdefs)
      {
        if (!mechdef.Description.Details.Contains("#Weapons:"))
        {
          StringBuilder stringBuilder = new StringBuilder("\n#Weapons:\n");
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          foreach (WeaponDef weaponDef in ((IEnumerable<MechComponentRef>) mechdef.Inventory).Where<MechComponentRef>((Func<MechComponentRef, bool>) (i => i.ComponentDefType == ComponentType.Weapon)).Select<MechComponentRef, WeaponDef>((Func<MechComponentRef, WeaponDef>) (i => i.Def as WeaponDef)))
          {
            if (dictionary.ContainsKey(weaponDef.Description.UIName))
              ++dictionary[weaponDef.Description.UIName];
            else
              dictionary[weaponDef.Description.UIName] = 1;
          }
          foreach (KeyValuePair<string, int> keyValuePair in dictionary)
            stringBuilder.Append(string.Format("  {0}x {1}\n", (object) keyValuePair.Value, (object) keyValuePair.Key));
          string str = stringBuilder.ToString();
          Traverse traverse1 = new Traverse((object) mechdef.Description);
          Traverse traverse2 = new Traverse((object) mechdef.Chassis.Description);
          traverse1.Property<string>("Details").Value = mechdef.Description.Details + str;
          traverse2.Property<string>("Details").Value = mechdef.Chassis.Description.Details + str;
        }
      }
    }

    public static void FixVehicleUIName(List<MechDef> mechdefs, SimGameState simgame)
    {
      if (UnityGameInstance.BattleTechGame.Simulation == null || UnityGameInstance.BattleTechGame.Simulation.DataManager == null)
        return;
      foreach (MechDef mechdef in mechdefs)
      {
        if (string.IsNullOrEmpty(mechdef.Description.UIName))
        {
          Traverse traverse1 = new Traverse((object) mechdef.Description);
          Traverse traverse2 = new Traverse((object) mechdef.Chassis.Description);
          VehicleDef vehicleDef = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(mechdef.Description.Id);
          UIName res;
          string str = !vehicleDef.Chassis.Is<UIName>(out res) || string.IsNullOrEmpty(res.N) ? vehicleDef.Description.Name : res.N;
          traverse1.Property<string>("UIName").Value = str;
          traverse2.Property<string>("UIName").Value = str;
        }
      }
    }
  }
}
