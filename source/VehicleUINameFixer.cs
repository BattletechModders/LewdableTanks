using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace LewdableTanks;

internal class VehicleUINameFixer : IMechDefProcessor
{
    public void ProcessMechDefs(List<MechDef> mechDefs)
    {
        foreach (var mechDef in mechDefs)
        {
            if (!string.IsNullOrEmpty(mechDef.Description.UIName))
                continue;

            if (!mechDef.IsVehicle())
            {
                continue;
            }

            var v = mechDef.dataManager.VehicleDefs.Get(mechDef.Description.Id);

            var name = (v.Chassis.Is<UIName>(out var uiName) && !string.IsNullOrEmpty(uiName.N))
                ? uiName.N
                : v.Description.Name;

            mechDef.Description.UIName = name;
            mechDef.Chassis.Description.UIName = name;
        }
    }
}