using Harmony;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (CustomShops.Control))]
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
