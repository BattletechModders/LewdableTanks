using BattleTech;
using CustomComponents;
using System.Collections.Generic;

namespace LewdableTanks {
  internal class VehicleCostFixer : IMechDefProcessor {
    public void ProcessMechDefs(List<MechDef> mechDefs) {
      float vehiclePartCostMult = Control.Instance.Settings.VehiclePartCostMult;
      foreach (MechDef mechDef in mechDefs) {
        if (mechDef.IsVehicle())
          mechDef.simGameMechPartCost = (int)((double)(mechDef.Description.Cost / 5) * (double)vehiclePartCostMult);
      }
    }
  }
}
