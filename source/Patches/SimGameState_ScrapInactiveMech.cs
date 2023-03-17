using BattleTech;
using CustomSalvage;
using UnityEngine;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("ScrapInactiveMech")]
public static class SimGameState_ScrapInactiveMech
{
    public static bool ScrapInactiveVehicle(string id, bool pay, SimGameState __instance, ref bool __result)
    {
        if (!__instance.DataManager.Exists(BattleTechResourceType.ChassisDef, id))
            return true;

        var def = __instance.DataManager.ChassisDefs.Get(id);
        if (!def.IsVehicle())
            return true;
            
        __result = false;
        if (__instance.GetItemCount(id, typeof(MechDef), SimGameState.ItemCountType.UNDAMAGED_ONLY) > 0)
        {
            __instance.RemoveItemStat(id, typeof(MechDef), false);
            __result = true;
            if (pay)
            {
                var mdef = __instance.DataManager.MechDefs.Get(ChassisHandler.GetMDefFromCDef(id));
                __instance.AddFunds(Mathf.RoundToInt((float)mdef.Description.Cost * __instance.Constants.Finances.MechScrapModifier), "Scrapping", true, true);

            }
        }
        return false;
    }

}