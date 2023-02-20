using BattleTech;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (MechBayPanel))]
  [HarmonyPatch("RefreshData")]
  public static class MechBayPanel_Init
  {
    [HarmonyPostfix]
    public static void show_all(MechBayPanel __instance)
    {
      SimGameState sim = __instance.Sim;
      if (!Control.Instance.Settings.DebugInfo.HasFlag((Enum) DInfo.Debug))
        return;
      Control.Instance.LogDebug(DInfo.Debug, "Player Mech:");
      foreach (KeyValuePair<int, MechDef> activeMech in sim.ActiveMechs)
        Control.Instance.LogDebug(DInfo.Debug, "-- {0:00}[{3}]:{1}/{2}", (object) activeMech.Key, (object) activeMech.Value.Description.Id, (object) activeMech.Value.Chassis?.Description.Id, (object) activeMech.Value.GUID);
      Control.Instance.LogDebug(DInfo.Debug, "Stored Mech:");
      foreach (ChassisDef inventoryMechDef in sim.GetAllInventoryMechDefs())
        Control.Instance.LogDebug(DInfo.Debug, "-- {0} ", (object) inventoryMechDef.Description.Id);
    }
  }
}
