using System.Collections.Generic;
using BattleTech;
using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("build_mech_tags")]
public static class ChassisHandler_build_mech_tags
{
    [HarmonyPrefix]
    public static bool build_vehicle_tags(MechDef mech, ref HashSet<string> __result)
    {
        var chassisDef = mech.Chassis;
        if (!chassisDef.IsVehicle())
            return true;

        __result = new HashSet<string>();
        if (mech.MechTags != null)
            __result.UnionWith(mech.MechTags);
        if (mech.Chassis.ChassisTags != null)
            __result.UnionWith(mech.Chassis.ChassisTags);


        return false;
    }
}