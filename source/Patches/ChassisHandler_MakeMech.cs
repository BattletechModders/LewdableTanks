using BattleTech;
using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (ChassisHandler))]
  [HarmonyPatch("MakeMech")]
  public static class ChassisHandler_MakeMech
  {
    public static bool in_work = false;
    public static bool empty = false;
    public static BrokeType broke;

    [HarmonyPrefix]
    public static void Prepare()
    {
      ChassisHandler_MakeMech.in_work = new Traverse(typeof (ChassisHandler)).Field<MechDef>("mech").Value.IsVehicle();
      if (!ChassisHandler_MakeMech.in_work)
        return;
      ChassisHandler_MakeMech.empty = CustomSalvage.Control.Instance.Settings.UnEquipedMech;
      ChassisHandler_MakeMech.broke = CustomSalvage.Control.Instance.Settings.MechBrokeType;
      CustomSalvage.Control.Instance.Settings.MechBrokeType = BrokeType.None;
      CustomSalvage.Control.Instance.Settings.UnEquipedMech = false;
    }

    [HarmonyPostfix]
    public static void Finish()
    {
      if (!ChassisHandler_MakeMech.in_work)
        return;
      CustomSalvage.Control.Instance.Settings.MechBrokeType = ChassisHandler_MakeMech.broke;
      CustomSalvage.Control.Instance.Settings.UnEquipedMech = ChassisHandler_MakeMech.empty;
    }
  }
}
