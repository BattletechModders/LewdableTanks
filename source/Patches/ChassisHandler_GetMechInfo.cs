using System;
using System.Linq;
using BattleTech;
using CustomComponents;
using CustomSalvage;
using UnityEngine;

namespace LewdableTanks.Patches;

//[HarmonyPatch(typeof(ChassisHandler))]
//[HarmonyPatch("GetMechInfo")]
public static class ChassisHandler_GetMechInfo
{
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static void Prefix(ref bool __runOriginal, MechDef mech, ref ChassisHandler.mech_info __result)
    {
        if (!__runOriginal)
        {
            return;
        }

        if (!mech.IsVehicle())
        {
            return;
        }

        __result = new ChassisHandler.mech_info();
        string id = mech.Description.Id;
        var vehicle = UnityGameInstance.BattleTechGame.Simulation.DataManager.VehicleDefs.Get(id);
        var css = CustomSalvage.Control.Instance.Settings;

        int max_parts = UnityGameInstance.BattleTechGame.Simulation.Constants.Story.DefaultMechPartMax;
        int min_parts_special = Mathf.CeilToInt(max_parts * css.MinPartsToAssemblySpecial);

        var assembly = vehicle.Chassis.GetComponent<VAssemblyVariant>();
        __result.Omni = !String.IsNullOrEmpty(Control.Instance.Settings.OmniTechTag) &&
                        vehicle.VehicleTags.Contains(Control.Instance.Settings.OmniTechTag);

        if (!Control.Instance.Settings.AllowFrankenTank)
            __result.Excluded = true;
        else if (assembly != null && assembly.Exclude)
            __result.Excluded = true;
        else if (assembly != null && assembly.Include)
            __result.Excluded = false;
        else
        if (css.ExcludeVariants.Contains(id))
            __result.Excluded = true;
        else if (css.ExcludeTags.Any(extag => vehicle.VehicleTags.Contains(extag)))
            __result.Excluded = true;

        if (css.SpecialTags != null && css.SpecialTags.Length > 0)
            foreach (var tag_info in css.SpecialTags)
            {
                if (vehicle.VehicleTags.Contains(tag_info.Tag))
                {
                    __result.MinParts = min_parts_special;
                    __result.PriceMult *= tag_info.Mod;
                    __result.Special = true;
                }
            }

        if (__result.Omni)
            __result.MinParts = 1;

        if (assembly != null)
        {
            if (assembly.ReplacePriceMult)
                __result.PriceMult = assembly.PriceMult;
            else
                __result.PriceMult *= assembly.PriceMult;

            if (assembly.PartsMin >= 0)
                __result.MinParts = Mathf.CeilToInt(max_parts * assembly.PartsMin);
        }

        Log.Main.Debug?.Log($"GetVehicle info for {vehicle.Chassis.Description.Id}");
        if (assembly != null)
        {
            Log.Main.Debug?.Log("-- " + assembly);
        }
        else
        {
            Log.Main.Debug?.Log($"-- VAssemblyVariant null, PID: {vehicle.Chassis.PrefabIdentifier}");
        }

        __result.PrefabID = ChassisHandler.GetPrefabId(mech);

        __runOriginal = false;
    }
}