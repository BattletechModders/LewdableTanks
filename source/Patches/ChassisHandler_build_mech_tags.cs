using System.Collections.Generic;
using BattleTech;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("build_mech_tags")]
public static class ChassisHandler_build_mech_tags
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mech, ref HashSet<string> __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        var chassisDef = mech.Chassis;
        if (!chassisDef.IsVehicle())
        {
            return;
        }

        __result = new HashSet<string>();
        if (mech.MechTags != null)
            __result.UnionWith(mech.MechTags);
        if (mech.Chassis.ChassisTags != null)
            __result.UnionWith(mech.Chassis.ChassisTags);


        __runOriginal = false;
    }
}