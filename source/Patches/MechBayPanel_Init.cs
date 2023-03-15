using BattleTech.UI;
using Harmony;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(MechBayPanel))]
    [HarmonyPatch("RefreshData")]
    public static class MechBayPanel_Init
    {
        [HarmonyPostfix]
        public static void show_all(MechBayPanel __instance)
        {
            var sim = __instance.Sim;

            if (!Control.Instance.Settings.DebugInfo.HasFlag(DInfo.Debug))
                return;


            Control.Instance.LogDebug(DInfo.Debug, "Player Mech:");
            foreach (var mech in sim.ActiveMechs)
            {
                Control.Instance.LogDebug(DInfo.Debug, "-- {0:00}[{3}]:{1}/{2}", mech.Key,
                    mech.Value.Description.Id, mech.Value.Chassis?.Description.Id, mech.Value.GUID);
            }
            Control.Instance.LogDebug(DInfo.Debug, "Stored Mech:");
            foreach (var mech in sim.GetAllInventoryMechDefs())
            {
                Control.Instance.LogDebug(DInfo.Debug, "-- {0} ", mech.Description.Id);
                //foreach (var tag in mech.ChassisTags)
                //    Control.Instance.LogDebug(DInfo.Debug, "-- -C:{0}", tag );
            }
        }
    }
}