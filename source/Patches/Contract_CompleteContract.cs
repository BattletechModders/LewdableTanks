using System.Linq;
using BattleTech;
using Harmony;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(Contract))]
    [HarmonyPatch("CompleteContract")]
    public static class Contract_CompleteContract
    {
        [HarmonyPostfix]
        public static void RepairTanks(Contract __instance)
        {
            void repair_location(MechDef mech, ChassisLocations location)
            {
                var locationLoadoutDef = mech.GetLocationLoadoutDef(location);
                var chassisLocationDef = mech.GetChassisLocationDef(location);

                locationLoadoutDef.CurrentArmor = locationLoadoutDef.AssignedArmor;
                locationLoadoutDef.CurrentInternalStructure = chassisLocationDef.InternalStructure;
            }

            void repair_torso_location(MechDef mech, ChassisLocations location)
            {
                var locationLoadoutDef = mech.GetLocationLoadoutDef(location);
                var chassisLocationDef = mech.GetChassisLocationDef(location);

                locationLoadoutDef.CurrentArmor = locationLoadoutDef.AssignedArmor;
                locationLoadoutDef.CurrentRearArmor = locationLoadoutDef.AssignedRearArmor;
                locationLoadoutDef.CurrentInternalStructure = chassisLocationDef.InternalStructure;
            }

            foreach (var mech in __instance.PlayerUnitResults.Where(i => !i.mechLost).Where(i => i.mech.IsVehicle()).Select(i => i.mech))
            {
                foreach (var item in mech.Inventory)
                    item.DamageLevel = ComponentDamageLevel.Functional;

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