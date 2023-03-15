using System;
using BattleTech;
using CustomSalvage;
using Harmony;
using UnityEngine;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(CustomSalvage.Contract_GenerateSalvage))]
    [HarmonyPatch("AddVechicleToSalvage")]
    public static class GenerateSalvage_AddVechicleToSalvage
    {
        [HarmonyPrefix]
        public static void AddVehiclePartsToSalvage(Vehicle vechicle, ContractHelper contract, SimGameState simgame)
        {
            try
            {
                var vid = vechicle.VehicleDef.Description.Id;
                var vdef = vechicle.VehicleDef;
                if (vdef == null)
                {
                    Log.Main.Error?.Log("No vehicledef for salvage");
                }

                if (simgame.DataManager.MechDefs.TryGet(vid, out var mech))
                {
                    if (!string.IsNullOrEmpty(Control.Instance.Settings.NoVehiclePartsTag))
                        if (vechicle.VehicleDef.VehicleTags.Contains(Control.Instance.Settings.NoVehiclePartsTag))
                        {
                            Log.Main.Debug?.Log($"Salvaging {vid} - no parts by tags");
                            return;
                        }

                    int min_parts = 1;
                    int max_parts = simgame.Constants.Story.DefaultMechPartMax;

                    var total = vechicle.SummaryArmorMax * Control.Instance.Settings.ArmorEffectOnHP + vechicle.SummaryStructureMax;
                    var current = vechicle.SummaryArmorCurrent * Control.Instance.Settings.ArmorEffectOnHP + vechicle.SummaryStructureCurrent;

                    var parts = Mathf.Clamp(Mathf.CeilToInt(current / total * max_parts), min_parts, max_parts);
                    Log.Main.Debug?.Log($"Salvaging {vid} - hp: {current:0.0}/{total:0.0} parts:{parts}");

                    contract.AddMechPartsToPotentialSalvage(simgame.Constants, mech, parts);
                }
                else
                    Log.Main.Error?.Log($"Cannot find fake mech for {vid}");
            }
            catch (Exception e)
            {
                Log.Main.Error?.Log("Error in Generate vehicle salvage: ", e);
            }
        }
    }
}
