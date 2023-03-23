using BattleTech;
using BattleTech.Data;
using CustomSalvage;
using HBS.Collections;

namespace LewdableTanks.Patches;

//[HarmonyPatch(typeof(MechBayChassisUnitElement_SetData))]
//[HarmonyPatch("GetMechTags")]
public static class MechBayChassisUnitElement_SetData_GetMechTags
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, ChassisDef chassis, DataManager dm, ref TagSet __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!chassis.IsVehicle())
        {
            return;
        }

        var mid = ChassisHandler.GetMDefFromCDef(chassis.Description.Id);
        var mech = dm.VehicleDefs.Get(mid);
        if (mech == null)
        { 
            Log.Main.Error?.Log("Cannot find vehicle with id " + mid);
            return;
        }


        __result = mech.VehicleTags;

        __runOriginal = false;
    }
}