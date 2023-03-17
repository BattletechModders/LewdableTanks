using BattleTech;
using HBS;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ShopDefItem))]
[HarmonyPatch("ToSalvageDef")]
public static class ShopDefItem_ToSalvageDef
{
    [HarmonyPrefix]
    public static bool ToVehicleSalvageDef(ref SalvageDef salvageDef, ShopDefItem __instance)
    {
        if (__instance.Type != ShopItemType.Mech)
            return true;

        var dataManager = SceneSingletonBehavior<UnityGameInstance>.Instance.Game.DataManager;
        string id = CustomSalvage.ChassisHandler.GetMDefFromCDef(__instance.GUID);
        MechDef mechDef3 = null;
        if (dataManager.MechDefs.Exists(id))
        {
            mechDef3 = dataManager.MechDefs.Get(id);
        }
        MechDef mechDef4 = new MechDef(mechDef3, null, true);
        mechDef4.Refresh();
        if (mechDef4 != null)
        {
            salvageDef.MechComponentDef = null;
            salvageDef.Description = mechDef3.Description;
            salvageDef.Type = SalvageDef.SalvageType.CHASSIS;
            salvageDef.ComponentType = ComponentType.MechPart;
        }
        return false;

    }
}