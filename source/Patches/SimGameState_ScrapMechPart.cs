using BattleTech;
using CustomSalvage;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (SimGameState))]
  [HarmonyPatch("ScrapMechPart")]
  public static class SimGameState_ScrapMechPart
  {
    [HarmonyPrefix]
    public static bool ScrapVehiclePart(
      string id,
      float partCount,
      float partMax,
      bool pay,
      SimGameState __instance,
      ref bool __result)
    {
      string mdefFromCdef = ChassisHandler.GetMDefFromCDef(id);
      Control.Instance.LogDebug(DInfo.General, "Scrapping {2}x{0}/{1}", (object) id, (object) mdefFromCdef, (object) partCount);
      __result = false;
      if (__instance.GetItemCount(mdefFromCdef, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0)
      {
        new Traverse((object) __instance).Method("RemoveItemStat", new System.Type[3]
        {
          typeof (string),
          typeof (string),
          typeof (bool)
        }, new object[3]
        {
          (object) mdefFromCdef,
          (object) "MECHPART",
          (object) false
        }).GetValue();
        __result = true;
        if (pay)
        {
          if (!__instance.DataManager.Exists(BattleTechResourceType.ChassisDef, id))
          {
            __result = false;
            return false;
          }
          int val = Mathf.RoundToInt((float) ((double) __instance.DataManager.ChassisDefs.Get(id).Description.Cost * (double) __instance.Constants.Finances.MechScrapModifier * ((double) partCount / (double) partMax)));
          __instance.AddFunds(val, "Scrapping");
        }
      }
      return false;
    }
  }
}
