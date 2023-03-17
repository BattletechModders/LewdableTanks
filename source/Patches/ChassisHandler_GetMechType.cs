using BattleTech;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("GetMechType")]
public static class ChassisHandler_GetMechType
{
    [HarmonyPrefix]
    public static bool GetVehicleType(MechDef mech, ref string __result)
    {
        if (!mech.IsVehicle())
            return true;

        __result = "Vehicle";
        return false;
    }
}