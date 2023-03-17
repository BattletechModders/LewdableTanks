using BattleTech;
using CustomComponents;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("GetPrefabId")]
public static class ChassisHandler_GetPrefabId
{
    [HarmonyPrefix]
    public static bool GetVehiclePrefabID(MechDef mech, ref string __result)
    {
        if (!mech.IsVehicle())
            return true;

        var vehicle = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(mech.Description.Id);
        var assembly = vehicle.Chassis.GetComponent<VAssemblyVariant>();
            
        __result = (assembly != null && !string.IsNullOrEmpty(assembly.PrefabID)
            ? assembly.PrefabID
            : vehicle.Chassis.PrefabIdentifier);

        if (Control.Instance.Settings.AddTonnageToPrefabID)
            __result += vehicle.Chassis.Tonnage.ToString();


        return false;
    }
}