using BattleTech;
using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(ChassisHandler))]
    [HarmonyPatch("MakeMech")]
    public static class ChassisHandler_MakeMech
    {
        public static bool in_work = false;
        public static bool empty = false;
        public static bool broke = false;

        [HarmonyPrefix]
        public static void Prepare()
        {
            var mech = new Traverse(typeof(ChassisHandler)).Field<MechDef>("mech").Value;
            in_work = mech.IsVehicle();
            if (in_work)
            {
                empty = CustomSalvage.Control.Instance.Settings.UnEquipedMech;
                broke = CustomSalvage.Control.Instance.Settings.BrokenMech;

                CustomSalvage.Control.Instance.Settings.BrokenMech = false;
                CustomSalvage.Control.Instance.Settings.UnEquipedMech = false;
            }
        }

        [HarmonyPostfix]
        public static void Finish()
        {
            if (in_work)
            {
                CustomSalvage.Control.Instance.Settings.BrokenMech = broke;
                CustomSalvage.Control.Instance.Settings.UnEquipedMech = empty;
            }
        }
    }
}