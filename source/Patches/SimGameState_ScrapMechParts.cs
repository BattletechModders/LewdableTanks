﻿using BattleTech;
using UnityEngine;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("ScrapMechPart")]
public static class SimGameState_ScrapMechPart
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, string id, float partCount, float partMax, bool pay,
        SimGameState __instance, ref bool __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mid = CustomSalvage.ChassisHandler.GetMDefFromCDef(id);
        Log.Main.Debug?.Log($"Scrapping {partCount}x{id}/{mid}");
        __result = false;
        if (__instance.GetItemCount(mid, "MECHPART", SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0)
        {
            __instance.RemoveItemStat(mid, "MECHPART", false);
            __result = true;
            if (pay)
            {
                if (!__instance.DataManager.Exists(BattleTechResourceType.ChassisDef, id))
                {
                    __result = false;
                    __runOriginal = false;
                    return;
                }
                int val = Mathf.RoundToInt((float)__instance.DataManager.ChassisDefs.Get(id).Description.Cost * __instance.Constants.Finances.MechScrapModifier * (partCount / partMax));
                __instance.AddFunds(val, "Scrapping", true, true);
            }
        }

        __runOriginal = false;
    }
}