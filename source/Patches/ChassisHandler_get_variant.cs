using BattleTech;
using CustomSalvage;
using CustomComponents;

namespace LewdableTanks.Patches;

[HarmonyPatch(typeof(ChassisHandler))]
[HarmonyPatch("get_variant")]
public static class ChassisHandler_get_variant
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mech, ref IAssemblyVariant __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!mech.IsVehicle())
        {
            return;
        }

        string id = mech.Description.Id;
        var vehicle = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(id);
        if(vehicle != null)
            __result = vehicle.Chassis.GetComponent<VAssemblyVariant>();

        __runOriginal = false;
    }

}