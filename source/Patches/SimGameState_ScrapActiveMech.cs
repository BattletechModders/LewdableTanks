using BattleTech;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("ScrapActiveMech")]
public class SimGameState_ScrapActiveMech
{
    [HarmonyPrefix]
    public static bool ScrapActiveVehicle(int baySlot, MechDef def, SimGameState __instance)
    {
        if (!def.IsVehicle())
            return true;
        if (def == null || (baySlot > 0 && !__instance.ActiveMechs.ContainsKey(baySlot)))
            return false;

        var locations = def.Locations;

        Log.Main.Trace?.Log($"Scrapping {def.Description.Id}");
        foreach (var location in locations)
        {
            Log.Main.Trace?.Log($"-- {location.Location} - {location.DamageLevel}");
        }

        if (__instance.ActiveMechs.ContainsKey(baySlot))
            __instance.ActiveMechs.Remove(baySlot);

        __instance.AddFunds(Mathf.RoundToInt((float)def.Description.Cost * __instance.Constants.Finances.MechScrapModifier), "Scrapping", true, true);
        return false;
    }
}