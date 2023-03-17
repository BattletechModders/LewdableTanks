using BattleTech.UI;

namespace LewdableTanks.SortFix;

[HarmonyPatch(typeof(ListElementController_SalvageMechPart_NotListView))]
[HarmonyPatch("GetName")]
public static class ListElementController_SalvageMechPart_NotListView_GetName
{
    [HarmonyPrefix]
    public static bool GetName(ListElementController_SalvageMechPart_NotListView __instance, ref string __result)
    {
        __result = __instance.salvageDef.Description.Name;
        return false;
    }
}