using BattleTech.UI;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(MechBayPanel))]
[HarmonyPatch("RefreshData")]
public static class MechBayPanel_Init
{
    [HarmonyPostfix]
    public static void show_all(MechBayPanel __instance)
    {
        var sim = __instance.Sim;

        if (Log.Main.Trace != null)
            return;


        Log.Main.Trace?.Log("Player Mech:");
        foreach (var mech in sim.ActiveMechs)
        {
            Log.Main.Trace?.Log(
                $"-- {mech.Key:00}[{mech.Value.GUID}]:{mech.Value.Description.Id}/{mech.Value.Chassis?.Description.Id}");
        }
        Log.Main.Trace?.Log("Stored Mech:");
        foreach (var mech in sim.GetAllInventoryMechDefs())
        {
            Log.Main.Trace?.Log($"-- {mech.Description.Id} ");
            //foreach (var tag in mech.ChassisTags)
            //    Log.Main.Trace?.Log("-- -C:{0}", tag );
        }
    }
}