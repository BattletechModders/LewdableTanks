
using BattleTech;
using CustomComponents;
using System.Collections.Generic;

namespace LewdableTanks {
  internal class VehicleUINameFixer : IMechDefProcessor {
    public void ProcessMechDefs(List<MechDef> mechDefs) {
      foreach (MechDef mechDef in mechDefs) {
        if (string.IsNullOrEmpty(mechDef.Description.UIName) && mechDef.IsVehicle()) {
          VehicleDef vehicleDef = mechDef.dataManager.VehicleDefs.Get(mechDef.Description.Id);
          UIName res;
          string str = !vehicleDef.Chassis.Is<UIName>(out res) || string.IsNullOrEmpty(res.N) ? vehicleDef.Description.Name : res.N;
          mechDef.Description.UIName = str;
          mechDef.Chassis.Description.UIName = str;
        }
      }
    }
  }
}
