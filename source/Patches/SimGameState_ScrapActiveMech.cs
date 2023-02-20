using BattleTech;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (SimGameState))]
  [HarmonyPatch("ScrapActiveMech")]
  public class SimGameState_ScrapActiveMech
  {
    [HarmonyPrefix]
    public static bool ScrapActiveVehicle(int baySlot, MechDef def, SimGameState __instance)
    {
      if (!def.IsVehicle())
        return true;
      if (def == null || baySlot > 0 && !__instance.ActiveMechs.ContainsKey(baySlot))
        return false;
      LocationLoadoutDef[] locationLoadoutDefArray = new Traverse((object) def).Field<LocationLoadoutDef[]>("Locations").Value;
      Control.Instance.LogDebug(DInfo.Debug, "Scrapping {0}", (object) def.Description.Id);
      foreach (LocationLoadoutDef locationLoadoutDef in locationLoadoutDefArray)
        Control.Instance.LogDebug(DInfo.Debug, "-- {0} - {1}", (object) locationLoadoutDef.Location, (object) locationLoadoutDef.DamageLevel);
      if (__instance.ActiveMechs.ContainsKey(baySlot))
        __instance.ActiveMechs.Remove(baySlot);
      __instance.AddFunds(Mathf.RoundToInt((float) def.Description.Cost * __instance.Constants.Finances.MechScrapModifier), "Scrapping");
      return false;
    }
  }
}
