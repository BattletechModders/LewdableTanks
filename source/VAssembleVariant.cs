using BattleTech;
using CustomComponents;
using CustomSalvage;

namespace LewdableTanks
{
    [CustomComponent("VAssemblyVariant")]
    public class VAssemblyVariant : SimpleCustom<VehicleChassisDef>, IAssemblyVariant
    {
        public string PrefabID { get; set; } = "";
        public bool Exclude { get; set; } = false;
        public bool Include { get; set; } = false;

       public bool ReplacePriceMult { get; set; } = false;
        public float PriceMult { get; set; } = 1f;
        public float PartsMin { get; set; } = -1;

        public override string ToString()
        {
            return $"VAssemblyVariant: (PID:[{PrefabID}], E:{Exclude}, I:{Include})";
        }
    }
}