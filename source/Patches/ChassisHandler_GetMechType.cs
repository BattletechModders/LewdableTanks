using BattleTech;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("GetMechType")]
public static class ChassisHandler_GetMechType
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mech, ref string __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!mech.IsVehicle())
        {
            return;
        }

        __result = "Vehicle";

        __runOriginal = false;
    }
}