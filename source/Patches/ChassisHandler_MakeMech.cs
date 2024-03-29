﻿using BattleTech;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("MakeMech")]
public static class ChassisHandler_MakeMech
{
    public static bool in_work = false;
    public static bool empty = false;
    public static BrokeType broke = BrokeType.None;

    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal)
    {
        if (!__runOriginal)
        {
            return;
        }

        var mech = new Traverse(typeof(ChassisHandler)).Field<MechDef>("mech").Value;
        in_work = mech.IsVehicle();
        if (in_work)
        {
            empty = CustomSalvage.Control.Instance.Settings.UnEquipedMech;
            broke = CustomSalvage.Control.Instance.Settings.MechBrokeType;

            CustomSalvage.Control.Instance.Settings.MechBrokeType = BrokeType.None;
            CustomSalvage.Control.Instance.Settings.UnEquipedMech = false;
        }
    }

    [HarmonyPostfix]
    [HarmonyWrapSafe]
    public static void Postfix()
    {
        if (in_work)
        {
            CustomSalvage.Control.Instance.Settings.MechBrokeType = broke;
            CustomSalvage.Control.Instance.Settings.UnEquipedMech = empty;
        }
    }
}