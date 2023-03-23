using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("GetMDefFromCDef")]
public static class ChassisHandler_GetMDefFromCDef
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref string __result, string cdefid)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!cdefid.StartsWith("vehiclechassisdef"))
        {
            return;
        }

        __result = cdefid.Replace("vehiclechassisdef", "vehicledef");
        __runOriginal = false;
    }
}

[HarmonyPatch(typeof(CustomShops.Control))]
[HarmonyPatch("GetMDefFromCDef")]
public static class Control_GetMDefFromCDef
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref string __result, string id)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!id.StartsWith("vehiclechassisdef"))
        {
            return;
        }

        __result = id.Replace("vehiclechassisdef", "vehicledef");
        __runOriginal = false;
    }
}