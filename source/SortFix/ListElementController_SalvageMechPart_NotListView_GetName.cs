using BattleTech.UI;

namespace LewdableTanks.SortFix;

[HarmonyPatch(typeof(ListElementController_SalvageMechPart_NotListView))]
[HarmonyPatch("GetName")]
public static class ListElementController_SalvageMechPart_NotListView_GetName
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ListElementController_SalvageMechPart_NotListView __instance, ref string __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        __result = __instance.salvageDef.Description.Name;

        __runOriginal = false;
    }
}