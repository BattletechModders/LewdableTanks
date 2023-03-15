using BattleTech;
using Harmony;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("UnreadyMech")]
public static class SimGameState_UnreadyMech
{
    [HarmonyPrefix]
    public static bool UnreadyVehicle(int baySlot, MechDef def, SimGameState __instance)
    {
        if (!def.IsVehicle())
            return true;
        if (def == null || (baySlot > 0 && !__instance.ActiveMechs.ContainsKey(baySlot)))
            return false;


        if (__instance.ActiveMechs.ContainsKey(baySlot))
            __instance.ActiveMechs.Remove(baySlot);

        __instance.AddItemStat(def.Chassis.Description.Id, def.GetType(), false);
        return false;
    }
}