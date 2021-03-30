using CustomUnits;
using Harmony;

namespace LewdableTanks.Patches
{

    [HarmonyPatch(typeof(MechBayMechInfoWidget_OnUnreadyClicked))]
    [HarmonyPatch("Prefix")]
    public static class MechBayMechInfoWidget_OnUnreadyClicked_Prefix
    {
        public static bool allowedit;

        [HarmonyPrefix]
        public static void CancelUnready()
        {
            allowedit = Core.Settings.AllowVehiclesEdit;
            Core.Settings.AllowVehiclesEdit = true;
        }

        [HarmonyPostfix]
        public static void Rollback()
        {
            Core.Settings.AllowVehiclesEdit = allowedit;
        }
    }
}