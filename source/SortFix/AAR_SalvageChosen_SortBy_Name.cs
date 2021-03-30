using System;
using BattleTech.UI;
using Harmony;

namespace LewdableTanks.SortFix
{
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

                i.LogDebug(DInfo.Sort, "SORT:");
                i.LogDebug(DInfo.Sort, "- a: {0}", a == null);

                i.LogDebug(DInfo.Sort, "-- a.controler:{0} a.salvageDef:{1}", a.controller == null, a.controller?.salvageDef);
                i.LogDebug(DInfo.Sort, "-- a.GetId:{0}", a.controller.GetId());
                i.LogDebug(DInfo.Sort, "-- a.GetName:{0}", a.controller.GetName());

                i.LogDebug(DInfo.Sort, "- b: {0}", a == null);

                i.LogDebug(DInfo.Sort, "-- b.controler:{0} b.salvageDef:{1}", b.controller == null, b.controller?.salvageDef);
                i.LogDebug(DInfo.Sort, "-- b.GetId:{0}", b.controller.GetId());
                i.LogDebug(DInfo.Sort, "-- b.GetName:{0}", b.controller.GetName());

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
}