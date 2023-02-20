using BattleTech;
using BattleTech.Data;
using CustomSalvage;
using Harmony;
using HBS;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (ShopDefItem))]
  [HarmonyPatch("ToSalvageDef")]
  public static class ShopDefItem_ToSalvageDef
  {
    [HarmonyPrefix]
    public static bool ToVehicleSalvageDef(ref SalvageDef salvageDef, ShopDefItem __instance)
    {
      if (__instance.Type != ShopItemType.Mech)
        return true;
      DataManager dataManager = SceneSingletonBehavior<UnityGameInstance>.Instance.Game.DataManager;
      string mdefFromCdef = ChassisHandler.GetMDefFromCDef(__instance.GUID);
      MechDef def = (MechDef) null;
      if (dataManager.MechDefs.Exists(mdefFromCdef))
        def = dataManager.MechDefs.Get(mdefFromCdef);
      MechDef mechDef = new MechDef(def);
      mechDef.Refresh();
      if (mechDef != null)
      {
        salvageDef.MechComponentDef = (MechComponentDef) null;
        salvageDef.Description = def.Description;
        salvageDef.Type = SalvageDef.SalvageType.CHASSIS;
        salvageDef.ComponentType = ComponentType.MechPart;
      }
      return false;
    }
  }
}
