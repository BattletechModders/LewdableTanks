using System;
using BattleTech;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(SimGameState))]
    [HarmonyPatch("ScrapMechPart")]
    public static class SimGameState_ScrapMechPart
    {
        [HarmonyPrefix]
        public static bool ScrapVehiclePart(string id, float partCount, float partMax, bool pay,
            SimGameState __instance, ref bool __result)
        {
            var mid = CustomSalvage.ChassisHandler.GetMDefFromCDef(id);
            Control.Instance.LogDebug(DInfo.General, "Scrapping {2}x{0}/{1}", id, mid, partCount);
            __result = false;
            if (__instance.GetItemCount(mid, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0)
            {
                var sim = new Traverse(__instance);
                sim.Method("RemoveItemStat", new Type[] {typeof(string), typeof(string), typeof(bool)},
                        new object[] {mid, "MECHPART", false}).GetValue();
                __result = true;
                if (pay)
                {
                    if (!__instance.DataManager.Exists(BattleTechResourceType.ChassisDef, id))
                    {
                        __result = false;
                        return false;
                    }
                    int val = Mathf.RoundToInt((float)__instance.DataManager.ChassisDefs.Get(id).Description.Cost * __instance.Constants.Finances.MechScrapModifier * (partCount / partMax));
                    __instance.AddFunds(val, "Scrapping", true, true);
                }
            }

            return false;
        }
    }
}