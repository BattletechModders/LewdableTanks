using BattleTech;
using BattleTech.Data;
using CustomSalvage;
using Harmony;
using HBS.Collections;

namespace LewdableTanks.Patches
{
  public static class MechBayChassisUnitElement_SetData_GetMechTags
  {
    [HarmonyPrefix]
    public static bool GetVehicleTags(ChassisDef chassis, DataManager dm, ref TagSet __result)
    {
      if (!chassis.IsVehicle())
        return true;
      string mdefFromCdef = ChassisHandler.GetMDefFromCDef(chassis.Description.Id);
      VehicleDef vehicleDef = dm.VehicleDefs.Get(mdefFromCdef);
      if (vehicleDef == null)
      {
        Control.Instance.LogError("Cannot find vehicle with id " + mdefFromCdef);
        return true;
      }
      __result = vehicleDef.VehicleTags;
      return false;
    }
  }
}
