using BattleTech;
using HBS;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ShopDefItem))]
[HarmonyPatch("ToSalvageDef")]
public static class ShopDefItem_ToSalvageDef
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ref SalvageDef salvageDef, ShopDefItem __instance)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (__instance.Type != ShopItemType.Mech)
        {
            return;
        }

        var dataManager = SceneSingletonBehavior<UnityGameInstance>.Instance.Game.DataManager;
        string id = CustomSalvage.ChassisHandler.GetMDefFromCDef(__instance.GUID);
        MechDef mechDef3 = null;
        if (dataManager.MechDefs.Exists(id))
        {
            mechDef3 = dataManager.MechDefs.Get(id);
        }
        MechDef mechDef4 = new MechDef(mechDef3, null, true);
        mechDef4.Refresh();
        salvageDef.MechComponentDef = null;
        salvageDef.Description = mechDef3.Description;
        salvageDef.Type = SalvageDef.SalvageType.CHASSIS;
        salvageDef.ComponentType = ComponentType.MechPart;

        __runOriginal = false;
    }
}