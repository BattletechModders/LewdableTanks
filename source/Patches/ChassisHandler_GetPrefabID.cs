using BattleTech;
using CustomComponents;
using CustomSalvage;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("GetPrefabId")]
public static class ChassisHandler_GetPrefabId
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mech, ref string __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!mech.IsVehicle())
        {
            return;
        }

        var vehicle = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(mech.Description.Id);
        var assembly = vehicle.Chassis.GetComponent<VAssemblyVariant>();
            
        __result = (assembly != null && !string.IsNullOrEmpty(assembly.PrefabID)
            ? assembly.PrefabID
            : vehicle.Chassis.PrefabIdentifier);

        if (Control.Instance.Settings.AddTonnageToPrefabID)
            __result += vehicle.Chassis.Tonnage.ToString();

        __runOriginal = false;
    }
}