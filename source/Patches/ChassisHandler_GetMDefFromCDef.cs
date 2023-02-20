using CustomSalvage;
using Harmony;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (ChassisHandler))]
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
}
