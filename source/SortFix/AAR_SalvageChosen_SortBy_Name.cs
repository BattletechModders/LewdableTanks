using System;
using BattleTech.UI;

namespace LewdableTanks.SortFix;

//[HarmonyPatch(typeof(AAR_SalvageChosen))]
//[HarmonyPatch("SortBy_Name")]
public static class AAR_SalvageChosen_SortBy_Name
{
    [HarmonyPrefix]
    public static void prefix(InventoryItemElement_NotListView a, InventoryItemElement_NotListView b)
    {
        try
        {
            var i = Control.Instance;

            Log.Main.Debug?.Log("SORT:");
            Log.Main.Debug?.Log($"- a: {a == null}");

            Log.Main.Debug?.Log($"-- a.controler:{a.controller == null} a.salvageDef:{a.controller?.salvageDef}");
            Log.Main.Debug?.Log($"-- a.GetId:{a.controller.GetId()}");
            Log.Main.Debug?.Log($"-- a.GetName:{a.controller.GetName()}");

            Log.Main.Debug?.Log($"- b: {a == null}");

            Log.Main.Debug?.Log($"-- b.controler:{b.controller == null} b.salvageDef:{b.controller?.salvageDef}");
            Log.Main.Debug?.Log($"-- b.GetId:{b.controller.GetId()}");
            Log.Main.Debug?.Log($"-- b.GetName:{b.controller.GetName()}");

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HarmonyPostfix]
    public static void postfix(int __result)
    {

    }

}