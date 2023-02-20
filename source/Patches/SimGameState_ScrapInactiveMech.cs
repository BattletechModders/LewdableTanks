using BattleTech;
using CustomSalvage;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (SimGameState))]
  [HarmonyPatch("ScrapInactiveMech")]
  public static class SimGameState_ScrapInactiveMech
  {
    public static bool ScrapInactiveVehicle(
      string id,
      bool pay,
      SimGameState __instance,
      ref bool __result)
    {
      if (!__instance.DataManager.Exists(BattleTechResourceType.ChassisDef, id) || !__instance.DataManager.ChassisDefs.Get(id).IsVehicle())
        return true;
      __result = false;
      if (__instance.GetItemCount(id, typeof (MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0)
      {
        __instance.RemoveItemStat(id, typeof (MechDef), false);
        __result = true;
        if (pay)
        {
          MechDef mechDef = __instance.DataManager.MechDefs.Get(ChassisHandler.GetMDefFromCDef(id));
          __instance.AddFunds(Mathf.RoundToInt((float) mechDef.Description.Cost * __instance.Constants.Finances.MechScrapModifier), "Scrapping");
        }
      }
      return false;
    }
  }
}
