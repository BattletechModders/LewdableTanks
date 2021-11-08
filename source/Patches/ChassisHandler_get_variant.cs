using BattleTech;
using CustomSalvage;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomComponents;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(ChassisHandler))]
    [HarmonyPatch("get_variant")]
    public static class ChassisHandler_get_variant
    {
        [HarmonyPrefix]
        public static bool get_vehicle_custom(MechDef mech, ref IAssemblyVariant __result)
        {
            if (!mech.IsVehicle())
                return true;

            string id = mech.Description.Id;
            var vehicle = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(id);
            if(vehicle != null)
                __result = vehicle.Chassis.GetComponent<VAssemblyVariant>();

            return false;
        }

    }
}
