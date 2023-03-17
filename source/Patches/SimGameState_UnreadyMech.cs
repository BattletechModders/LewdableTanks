using BattleTech;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SimGameState))]
[HarmonyPatch("UnreadyMech")]
public static class SimGameState_UnreadyMech
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, int baySlot, MechDef def, SimGameState __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!def.IsVehicle())
        {
            return;
        }

        if (def == null || (baySlot > 0 && !__instance.ActiveMechs.ContainsKey(baySlot)))
        {
            __runOriginal = false;
            return;
        }


        if (__instance.ActiveMechs.ContainsKey(baySlot))
            __instance.ActiveMechs.Remove(baySlot);

        __instance.AddItemStat(def.Chassis.Description.Id, def.GetType(), false);
        __runOriginal = false;
    }
}