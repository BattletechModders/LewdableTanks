using BattleTech;

namespace LewdableTanks;

public static class Extensions
{
    public static bool IsVehicle(this MechDef mech)
    {
        if (mech == null)
        {
            return false;
        }

        if (mech.MechTags != null && mech.MechTags.Contains("fake_vehicle"))
        {
            return true;
        }

        return IsVehicle(mech.Chassis);
    }

    public static bool IsVehicle(this ChassisDef chassis)
    {
        if (chassis == null)
        {
            return false;
        }

        return chassis.ChassisTags != null && chassis.ChassisTags.Contains("fake_vehicle_chassis");
    }

    private static string NoSalvageTag => CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag;
    internal static bool IsNoSalvage(this MechDef mech)
    {
        if (mech == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(NoSalvageTag))
        {
            return false;
        }

        return mech.MechTags != null && mech.MechTags.Contains(NoSalvageTag);
    }

    private static string NoVehiclePartsTag => Control.Instance.Settings.NoVehiclePartsTag;
    internal static bool IsNoVehicleParts(this MechDef mech)
    {
        if (mech == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(NoVehiclePartsTag))
        {
            return false;
        }

        return mech.MechTags != null && mech.MechTags.Contains(NoVehiclePartsTag);
    }
}