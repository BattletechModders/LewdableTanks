using BattleTech;
using CustomComponents;
using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (ChassisHandler))]
  [HarmonyPatch("GetPrefabId")]
  public static class ChassisHandler_GetPrefabId
  {
    [HarmonyPrefix]
    public static bool GetVehiclePrefabID(MechDef mech, ref string __result)
    {
      if (!mech.IsVehicle())
        return true;
      VehicleDef vehicleDef = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(mech.Description.Id);
      VAssemblyVariant component = VehicleExtentions.GetComponent<VAssemblyVariant>(vehicleDef.Chassis);
      __result = component == null || string.IsNullOrEmpty(component.PrefabID) ? vehicleDef.Chassis.PrefabIdentifier : component.PrefabID;
      if (LewdableTanks.Control.Instance.Settings.AddTonnageToPrefabID)
        __result += vehicleDef.Chassis.Tonnage.ToString();
      return false;
    }
  }
}
