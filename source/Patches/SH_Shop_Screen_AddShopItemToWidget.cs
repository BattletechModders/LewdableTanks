using BattleTech;
using BattleTech.UI;
using CustomSalvage;
using Harmony;
using UnityEngine.Events;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(SG_Shop_Screen))]
[HarmonyPatch("AddShopItemToWidget")]
[HarmonyAfter("CustomShops")]
public static class SH_Shop_Screen_AddShopItemToWidget
{
    [HarmonyPrefix]
    public static bool FixGetMechdef(ref ShopDefItem itemDef, Shop shop, bool isBulkAdd, bool isSelling, IMechLabDropTarget targetWidget,
        MechLabInventoryWidget_ListView ___inventoryWidget, bool ___isInBuyingState, SimGameState ___simState,
        SG_Shop_Screen __instance)
    {
        if (itemDef.Type == ShopItemType.Mech && !___isInBuyingState)
        {
            var dataManager = ___simState.DataManager;

            string guid8 = itemDef.GUID;
            if (dataManager.ChassisDefs.Exists(guid8))
            {
                ChassisDef chassisDef = dataManager.ChassisDefs.Get(guid8);
                string newGUID = ___simState.GenerateSimGameUID();
                var id = ChassisHandler.GetMDefFromCDef(guid8);
                MechDef stockMech = dataManager.MechDefs.Get(id);
                MechDef mechDef3 = new MechDef(chassisDef, newGUID, stockMech);
                mechDef3.Refresh();
                if (mechDef3 != null)
                {
                    InventoryDataObject_ShopFullMech inventoryDataObject_ShopFullMech2 = new InventoryDataObject_ShopFullMech();
                    inventoryDataObject_ShopFullMech2.Init(mechDef3, itemDef, shop, ___simState, dataManager,
                        targetWidget, itemDef.Count, isSelling, new UnityAction<InventoryItemElement>(__instance.OnItemSelected));
                    ___inventoryWidget.AddItemToInventory(inventoryDataObject_ShopFullMech2, isBulkAdd);
                    inventoryDataObject_ShopFullMech2.SetItemDraggable(false);
                }
            }
            return false;
        }

        return true;
    }
}