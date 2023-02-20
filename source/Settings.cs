namespace LewdableTanks
{
  public class Settings
  {
    public string NoVehiclePartsTag = "";

    public bool AddLogPrefix { get; set; } = true;

    public DInfo DebugInfo { get; set; } = DInfo.ALL;

    public bool ShowSettingsOnLoad { get; set; } = true;

    public string FakeVehicleTag { get; set; } = "fake_vehicle_chassis";

    public PlayerVehicleAction LostVehicleAction { get; set; } = PlayerVehicleAction.Salvage;

    public float ModuleRecoveryChance { get; set; } = 0.5f;

    public bool FixMechPartCost { get; set; } = true;

    public bool FixUIName { get; set; } = true;

    public bool AddWeaponToDescription { get; set; } = true;

    public bool AddTonnageToPrefabID { get; set; } = true;

    public PlayerVehicleRecoveryType Recovery { get; set; } = PlayerVehicleRecoveryType.HpLeftConstant;

    public float RecoveryChanceConstantMod { get; set; } = -0.5f;

    public float RecoveryChanceHPMod { get; set; } = 0.5f;

    public float RecoveryChanceHPBase { get; set; } = -0.5f;

    public string OmniTechTag { get; set; } = "omni_tank";

    public bool AllowFrankenTank { get; set; } = true;

    public float VehiclePartCostMult { get; set; } = 1f;
  }
}
