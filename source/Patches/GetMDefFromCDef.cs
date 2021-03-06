using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(ChassisHandler))]
    [HarmonyPatch("GetMDefFromCDef")]
    public static class ChassisHandler_GetMDefFromCDef
    {
        [HarmonyPrefix]
        public static bool GetVDefFromCDef(ref string __result, string cdefid)
        {

            if (!cdefid.StartsWith("vehiclechassisdef"))
                return true;

            __result = cdefid.Replace("vehiclechassisdef", "vehicledef");
            return false;
        }
    }

    [HarmonyPatch(typeof(CustomShops.Control))]
    [HarmonyPatch("GetMDefFromCDef")]
    public static class Control_GetMDefFromCDef
    {
        [HarmonyPrefix]
        public static bool GetVDefFromCDef(ref string __result, string id)
        {
            if (!id.StartsWith("vehiclechassisdef"))
                return true;

            __result = id.Replace("vehiclechassisdef", "vehicledef");
            return false;
        }
    }
}
