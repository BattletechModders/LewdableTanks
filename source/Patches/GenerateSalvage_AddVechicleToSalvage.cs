using BattleTech;
using CustomSalvage;
using Harmony;
using System;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (Contract_GenerateSalvage))]
  [HarmonyPatch("AddVechicleToSalvage")]
  public static class GenerateSalvage_AddVechicleToSalvage
  {
    [HarmonyPrefix]
    public static void AddVehiclePartsToSalvage(
      Vehicle vechicle,
      ContractHelper contract,
      SimGameState simgame)
    {
      try
      {
        string id = vechicle.VehicleDef.Description.Id;
        if (vechicle.VehicleDef == null)
          Control.Instance.LogError("No vehicledef for salvage");
        MechDef t;
        if (simgame.DataManager.MechDefs.TryGet(id, out t))
        {
          if (!string.IsNullOrEmpty(Control.Instance.Settings.NoVehiclePartsTag) && vechicle.VehicleDef.VehicleTags.Contains(Control.Instance.Settings.NoVehiclePartsTag))
          {
            Control.Instance.LogDebug(DInfo.Salvage, "Salvaging {0} - no parts by tags", (object) id);
          }
          else
          {
            int min = 1;
            int defaultMechPartMax = simgame.Constants.Story.DefaultMechPartMax;
            float num1 = vechicle.SummaryArmorMax + vechicle.SummaryStructureMax;
            float num2 = vechicle.SummaryArmorCurrent + vechicle.SummaryStructureCurrent;
            int num3 = Mathf.Clamp(Mathf.CeilToInt(num2 / num1 * (float) defaultMechPartMax), min, defaultMechPartMax);
            Control.Instance.LogDebug(DInfo.Salvage, "Salvaging {0} - hp: {1:0.0}/{2:0.0} parts:{3}", (object) id, (object) num2, (object) num1, (object) num3);
            contract.AddMechPartsToPotentialSalvage(simgame.Constants, t, num3);
          }
        }
        else
          Control.Instance.LogError("Cannot find fake mech for " + id);
      }
      catch (Exception ex)
      {
        Control.Instance.LogError("Error in Generate vehicle salvage: ", ex);
      }
    }
  }
}
