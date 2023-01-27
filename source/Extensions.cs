using BattleTech;

namespace LewdableTanks
{
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
    }
}