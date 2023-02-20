using BattleTech;
using CustomSalvage;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (Contract_GenerateSalvage))]
  [HarmonyPatch("AddMechToSalvage")]
  public static class GenerateSalvage_AddMechToSalvage_asVehicle
  {
    private static HashSet<ChassisLocations> vehicleLocations = new HashSet<ChassisLocations>()
    {
      ChassisLocations.Head,
      ChassisLocations.LeftArm,
      ChassisLocations.RightArm,
      ChassisLocations.LeftLeg,
      ChassisLocations.RightLeg
    };

    [HarmonyPrefix]
    public static bool AddMechToSalvageAsVehicle(
      MechDef mech,
      ContractHelper contract,
      SimGameState simgame,
      SimGameConstants constants,
      bool can_upgrade)
    {
      Control.Instance.LogDebug(DInfo.Salvage, "-- Salvaging " + mech.Name);
      if (!mech.IsVehicle())
      {
        Control.Instance.LogDebug(DInfo.Salvage, "-- not a vehicle. returning to normal process");
        return true;
      }
      try
      {
        if (!string.IsNullOrEmpty(Control.Instance.Settings.NoVehiclePartsTag) && mech.MechTags.Contains(Control.Instance.Settings.NoVehiclePartsTag))
        {
          Control.Instance.LogDebug(DInfo.Salvage, "Salvaging {0} - no parts by tags", (object) mech.Description.Id);
        }
        else
        {
          int min = 1;
          int defaultMechPartMax = simgame.Constants.Story.DefaultMechPartMax;
          float num1 = 0.0f;
          float num2 = 0.0f;
          foreach (ChassisLocations vehicleLocation in GenerateSalvage_AddMechToSalvage_asVehicle.vehicleLocations)
          {
            LocationDef chassisLocationDef = mech.GetChassisLocationDef(vehicleLocation);
            LocationLoadoutDef locationLoadoutDef = mech.GetLocationLoadoutDef(vehicleLocation);
            if ((double) chassisLocationDef.InternalStructure > 1.0 || (double) chassisLocationDef.MaxArmor != 0.0)
            {
              num1 += locationLoadoutDef.AssignedArmor + chassisLocationDef.InternalStructure;
              num2 += locationLoadoutDef.CurrentArmor + locationLoadoutDef.CurrentInternalStructure;
              Control.Instance.LogDebug(DInfo.Salvage, "  -- location:" + (object) vehicleLocation + " AssignedArmor:" + (object) locationLoadoutDef.AssignedArmor + " InternalStructure:" + (object) chassisLocationDef.InternalStructure + " CurrentArmor:" + (object) locationLoadoutDef.CurrentArmor + " CurrentInternalStructure:" + (object) locationLoadoutDef.CurrentInternalStructure);
            }
          }
          if ((double) num1 == 0.0)
            num1 = 1f;
          int num3 = Mathf.Clamp(Mathf.CeilToInt(num2 / num1 * (float) defaultMechPartMax), min, defaultMechPartMax);
          Control.Instance.LogDebug(DInfo.Salvage, "Salvaging {0} - hp: {1:0.0}/{2:0.0} parts:{3}", (object) mech.Description.Id, (object) num2, (object) num1, (object) num3);
          contract.AddMechPartsToPotentialSalvage(simgame.Constants, mech, num3);
        }
      }
      catch (Exception ex)
      {
        Control.Instance.LogError("Error in adding parts", ex);
      }
      try
      {
        foreach (MechComponentRef mechComponentRef in ((IEnumerable<MechComponentRef>) mech.Inventory).Where<MechComponentRef>((Func<MechComponentRef, bool>) (item => !mech.IsLocationDestroyed(item.MountedLocation) && item.DamageLevel != ComponentDamageLevel.Destroyed)))
          contract.AddComponentToPotentialSalvage(mechComponentRef.Def, ComponentDamageLevel.Functional, can_upgrade);
      }
      catch (Exception ex)
      {
        Control.Instance.LogError("Error in adding component", ex);
      }
      return false;
    }
  }
}
