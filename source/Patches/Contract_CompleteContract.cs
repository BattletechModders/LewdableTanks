using BattleTech;
using Harmony;
using System;
using System.Linq;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (Contract))]
  [HarmonyPatch("CompleteContract")]
  public static class Contract_CompleteContract
  {
    internal static void repair_location(MechDef mech, ChassisLocations location) {
      LocationLoadoutDef locationLoadoutDef = mech.GetLocationLoadoutDef(location);
      LocationDef chassisLocationDef = mech.GetChassisLocationDef(location);
      locationLoadoutDef.CurrentArmor = locationLoadoutDef.AssignedArmor;
      locationLoadoutDef.CurrentInternalStructure = chassisLocationDef.InternalStructure;
    }

    internal static void repair_torso_location(MechDef mech, ChassisLocations location) {
      LocationLoadoutDef locationLoadoutDef = mech.GetLocationLoadoutDef(location);
      LocationDef chassisLocationDef = mech.GetChassisLocationDef(location);
      locationLoadoutDef.CurrentArmor = locationLoadoutDef.AssignedArmor;
      locationLoadoutDef.CurrentRearArmor = locationLoadoutDef.AssignedRearArmor;
      locationLoadoutDef.CurrentInternalStructure = chassisLocationDef.InternalStructure;
    }
    [HarmonyPostfix]
    public static void RepairTanks(Contract __instance)
    {
      foreach (MechDef mech in __instance.PlayerUnitResults.Where<UnitResult>((Func<UnitResult, bool>) (i => !i.mechLost)).Where<UnitResult>((Func<UnitResult, bool>) (i => i.mech.IsVehicle())).Select<UnitResult, MechDef>((Func<UnitResult, MechDef>) (i => i.mech)))
      {
        foreach (BaseComponentRef baseComponentRef in mech.Inventory)
          baseComponentRef.DamageLevel = ComponentDamageLevel.Functional;
        repair_location(mech, ChassisLocations.Head);
        repair_location(mech, ChassisLocations.LeftArm);
        repair_location(mech, ChassisLocations.LeftLeg);
        repair_location(mech, ChassisLocations.RightArm);
        repair_location(mech, ChassisLocations.RightLeg);
        repair_torso_location(mech, ChassisLocations.CenterTorso);
        repair_torso_location(mech, ChassisLocations.LeftTorso);
        repair_torso_location(mech, ChassisLocations.RightTorso);
      }
    }
  }
}
