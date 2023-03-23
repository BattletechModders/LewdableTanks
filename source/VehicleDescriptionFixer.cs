using System.Collections.Generic;
using System.Linq;
using System.Text;
using BattleTech;
using CustomComponents;

namespace LewdableTanks;

internal class VehicleDescriptionFixer : IMechDefProcessor
{
    public void ProcessMechDefs(List<MechDef> mechDefs)
    {
        foreach (var mechDef in mechDefs)
        {
            if (!mechDef.IsVehicle())
            {
                continue;
            }

            if (!mechDef.Description.Details.Contains("#Weapons:"))
            {
                var sb = new StringBuilder("\n#Weapons:\n");
                var d = new Dictionary<string, int>();
                foreach (var item in mechDef.Inventory
                             .Where(i => i.ComponentDefType == ComponentType.Weapon)
                             .Select(i => i.Def as WeaponDef))
                {
                    if (d.ContainsKey(item.Description.UIName))
                    {
                        d[item.Description.UIName] += 1;
                    }
                    else
                    {
                        d[item.Description.UIName] = 1;
                    }
                }

                foreach (var pair in d)
                {
                    sb.Append($"  {pair.Value}x {pair.Key}\n");
                }

                var str = sb.ToString();

                mechDef.Description.Details += str;
                mechDef.Chassis.Description.Details += str;
            }
        }
    }
}