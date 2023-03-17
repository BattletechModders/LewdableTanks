using CustomUnits;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(MechBayMechInfoWidget_OnUnreadyClicked))]
[HarmonyPatch("Prefix")]
public static class MechBayMechInfoWidget_OnUnreadyClicked_Prefix
{
    public static bool allowedit;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return;
        }

        allowedit = Core.Settings.AllowVehiclesEdit;
        Core.Settings.AllowVehiclesEdit = true;
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix()
    {
        Core.Settings.AllowVehiclesEdit = allowedit;
    }
}