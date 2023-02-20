using BattleTech;
using CustomComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LewdableTanks {
  internal class VehicleDescriptionFixer : IMechDefProcessor {
    public void ProcessMechDefs(List<MechDef> mechDefs) {
      foreach (MechDef mechDef in mechDefs) {
        if (mechDef.IsVehicle() && !mechDef.Description.Details.Contains("#Weapons:")) {
          StringBuilder stringBuilder = new StringBuilder("\n#Weapons:\n");
          Dictionary<string, int> dictionary = new Dictionary<string, int>();
          foreach (WeaponDef weaponDef in ((IEnumerable<MechComponentRef>)mechDef.Inventory).Where<MechComponentRef>((Func<MechComponentRef, bool>)(i => i.ComponentDefType == ComponentType.Weapon)).Select<MechComponentRef, WeaponDef>((Func<MechComponentRef, WeaponDef>)(i => i.Def as WeaponDef))) {
            if (dictionary.ContainsKey(weaponDef.Description.UIName))
              ++dictionary[weaponDef.Description.UIName];
            else
              dictionary[weaponDef.Description.UIName] = 1;
          }
          foreach (KeyValuePair<string, int> keyValuePair in dictionary)
            stringBuilder.Append(string.Format("  {0}x {1}\n", (object)keyValuePair.Value, (object)keyValuePair.Key));
          string str = stringBuilder.ToString();
          mechDef.Description.Details += str;
          mechDef.Chassis.Description.Details += str;
        }
      }
    }
  }
}
