using BattleTech;
using CustomComponents;

namespace LewdableTanks
{
    [CustomComponent("VAssemblyVariant")]
    public class VAssemblyVariant : SimpleCustom<VehicleChassisDef>
    {
        public string PrefabID = "";
        public bool Exclude = false;
        public bool Include = false;

        //public bool Special = false;
        //public bool CanUseSpecial = false;
        //public bool CanUseOnNormal = false;

        public bool ReplacePriceMult = false;
        public float PriceMult = 1f;
        public float PartsMin = -1;

        public override string ToString()
        {
            return $"VAssemblyVariant: (PID:[{PrefabID}], E:{Exclude}, I:{Include})";
        }
    }
}