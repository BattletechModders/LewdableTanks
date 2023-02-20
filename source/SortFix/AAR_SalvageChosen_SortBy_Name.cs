using BattleTech.UI;
using Harmony;
using System;

namespace LewdableTanks.SortFix
{
  public static class AAR_SalvageChosen_SortBy_Name
  {
    [HarmonyPrefix]
    public static void prefix(
      InventoryItemElement_NotListView a,
      InventoryItemElement_NotListView b)
    {
      try
      {
        Control instance = Control.Instance;
        instance.LogDebug(DInfo.Sort, "SORT:");
        instance.LogDebug(DInfo.Sort, "- a: {0}", (object) ((UnityEngine.Object) a == (UnityEngine.Object) null));
        instance.LogDebug(DInfo.Sort, "-- a.controler:{0} a.salvageDef:{1}", (object) (a.controller == null), (object) a.controller?.salvageDef);
        instance.LogDebug(DInfo.Sort, "-- a.GetId:{0}", (object) a.controller.GetId());
        instance.LogDebug(DInfo.Sort, "-- a.GetName:{0}", (object) a.controller.GetName());
        instance.LogDebug(DInfo.Sort, "- b: {0}", (object) ((UnityEngine.Object) a == (UnityEngine.Object) null));
        instance.LogDebug(DInfo.Sort, "-- b.controler:{0} b.salvageDef:{1}", (object) (b.controller == null), (object) b.controller?.salvageDef);
        instance.LogDebug(DInfo.Sort, "-- b.GetId:{0}", (object) b.controller.GetId());
        instance.LogDebug(DInfo.Sort, "-- b.GetName:{0}", (object) b.controller.GetName());
      }
      catch (Exception ex)
      {
        Console.WriteLine((object) ex);
        throw;
      }
    }

    [HarmonyPostfix]
    public static void postfix(int __result)
    {
    }
  }
}
