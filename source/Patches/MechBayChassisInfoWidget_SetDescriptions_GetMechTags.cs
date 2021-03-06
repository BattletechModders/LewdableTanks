using System.Runtime.InteropServices.WindowsRuntime;
using BattleTech;
using BattleTech.Data;
using BattleTech.UI;
using CustomSalvage;
using Harmony;
using HBS.Collections;

namespace LewdableTanks.Patches
{
    //[HarmonyPatch(typeof(MechBayChassisUnitElement_SetData))]
    //[HarmonyPatch("GetMechTags")]
    public static class MechBayChassisUnitElement_SetData_GetMechTags
    {
        [HarmonyPrefix]
        public static bool GetVehicleTags(ChassisDef chassis, DataManager dm, ref TagSet __result)
        {
            if (!chassis.IsVehicle())
                return true;

            var mid = ChassisHandler.GetMDefFromCDef(chassis.Description.Id);
            var mech = dm.VehicleDefs.Get(mid);
            if (mech == null)
            { 
                Control.Instance.LogError("Cannot find vehicle with id " + mid);
                return true;
            }


            __result = mech.VehicleTags;

            return false;
        }
    }
}