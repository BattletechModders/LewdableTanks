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
    public bool ReplacePriceMult = false;
    public float PriceMult = 1f;
    public float PartsMin = -1f;

    public override string ToString() => string.Format("VAssemblyVariant: (PID:[{0}], E:{1}, I:{2})", (object) this.PrefabID, (object) this.Exclude, (object) this.Include);
  }
}
