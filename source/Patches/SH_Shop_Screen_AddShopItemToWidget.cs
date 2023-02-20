using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomSalvage;
using Harmony;
using UnityEngine.Events;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (SG_Shop_Screen))]
  [HarmonyPatch("AddShopItemToWidget")]
  [HarmonyAfter(new string[] {"CustomShops"})]
  public static class SH_Shop_Screen_AddShopItemToWidget
  {
    [HarmonyPrefix]
    public static bool FixGetMechdef(
      ref ShopDefItem itemDef,
      Shop shop,
      bool isBulkAdd,
      bool isSelling,
      IMechLabDropTarget targetWidget,
      MechLabInventoryWidget_ListView ___inventoryWidget,
      bool ___isInBuyingState,
      SimGameState ___simState,
      SG_Shop_Screen __instance)
    {
      if (itemDef.Type != ShopItemType.Mech || ___isInBuyingState)
        return true;
      DataManager dataManager = ___simState.DataManager;
      string guid = itemDef.GUID;
      if (dataManager.ChassisDefs.Exists(guid))
      {
        ChassisDef chassis = dataManager.ChassisDefs.Get(guid);
        string simGameUid = ___simState.GenerateSimGameUID();
        string mdefFromCdef = ChassisHandler.GetMDefFromCDef(guid);
        MechDef stockMech = dataManager.MechDefs.Get(mdefFromCdef);
        MechDef theMechDef = new MechDef(chassis, simGameUid, stockMech);
        theMechDef.Refresh();
        if (theMechDef != null)
        {
          InventoryDataObject_ShopFullMech objectShopFullMech = new InventoryDataObject_ShopFullMech();
          objectShopFullMech.Init(theMechDef, itemDef, shop, ___simState, dataManager, targetWidget, itemDef.Count, isSelling, new UnityAction<InventoryItemElement>(__instance.OnItemSelected));
          ___inventoryWidget.AddItemToInventory((InventoryDataObject_BASE) objectShopFullMech, isBulkAdd);
          objectShopFullMech.SetItemDraggable(false);
        }
      }
      return false;
    }
  }
}
