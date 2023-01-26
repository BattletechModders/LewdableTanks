using System;

namespace LewdableTanks
{
    [Flags]
    public enum DInfo
    {
        NONE = 0,
        General = 1,
        Salvage = 1 << 1,
        Sort = 1 << 2,
        AutoFix = 1 << 3,
        Death = 1 << 4,
        Assembly = 1 << 5,

        Debug = 0x1000000,

        ALL = 0x7fffffff,
    }

    public enum PlayerVehicleAction
    {
        None,
        Salvage,
        Return,
        SalvageParts,
        ReturnParts
    }

    public enum PlayerVehicleRecoveryType
    {
        NoRecovery,
        SimGameConstant,
        AlwaysRecovery,
        HpLeft,
        HpLeftConstant
    }

    public class Settings
    {
        public bool AddLogPrefix { get; set; } = true;
        public DInfo DebugInfo { get; set; } = DInfo.ALL | DInfo.Debug;
        public bool ShowSettingsOnLoad { get; set; } = true;

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
        public float ArmorEffectOnHP {get; set; } = 1f;

        public string OmniTechTag { get; set; } = "omni_tank";

        public bool AllowFrankenTank { get; set; } = true;

        public float VehiclePartCostMult { get; set; } = 1;

        public string NoVehiclePartsTag = "";
    }
}
