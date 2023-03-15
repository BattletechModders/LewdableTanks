using System.Collections.Generic;
using BattleTech;
using CustomComponents;

namespace LewdableTanks;

internal class VehicleCostFixer : IMechDefProcessor
{
    public void ProcessMechDefs(List<MechDef> mechDefs)
    {
        var k = Control.Instance.Settings.VehiclePartCostMult;

        foreach (var mechDef in mechDefs)
        {
            if (!mechDef.IsVehicle())
            {
                continue;
            }
            mechDef.simGameMechPartCost = (int)(mechDef.Description.Cost / 5 * k);
            Log.Main.Debug?.Log($"Fixing cost of {mechDef.Description.Id} set to {mechDef.SimGameMechPartCost}");
        }
    }
}