using BattleTech;
using CustomSalvage;
using Harmony;
using System.Linq;
using UnityEngine;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (Contract_GenerateSalvage))]
  [HarmonyPatch("ProccessPlayerMech")]
  public static class Contract_GenerateSalvage_ProccessPlayerMech
  {
    [HarmonyPrefix]
    public static bool ProcessPlayerVehicle(UnitResult unitResult, ContractHelper Contract)
    {
      MechDef mech1 = unitResult.mech;
      if (LewdableTanks.Control.Instance.Settings.LostVehicleAction == PlayerVehicleAction.None)
      {
        LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "- None action for player vehicle, skipping");
        return true;
      }
      if (!UnityGameInstance.BattleTechGame.DataManager.MechDefs.Get(mech1.Description.Id).IsVehicle())
      {
        LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "- " + mech1.Description.Id + " with GUID " + mech1.GUID + " is not vehicle, return to mech process");
        return true;
      }
      LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- vehicles:");
      Mech vehicle = (Mech) null;
      foreach (Mech mech2 in Contract.Contract.BattleTechGame.Combat.AllActors.OfType<Mech>())
      {
        if (mech2.PilotableActorDef.GUID == mech1.GUID)
        {
          vehicle = mech2;
          break;
        }
      }
      if (vehicle == null)
      {
        LewdableTanks.Control.Instance.LogError("Vehicle " + mech1.Description.Id + " with GUID " + mech1.GUID + " not found, return vehicle to player fallback");
        unitResult.mechLost = false;
        return false;
      }
      unitResult.mechLost = !Contract_GenerateSalvage_ProccessPlayerMech.CanRecoverVehicle((AbstractActor) vehicle);
      LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "- " + mech1.Description.Id + " with GUID " + mech1.GUID + " found. Is lost:" + unitResult.mechLost.ToString());
      if (unitResult.mechLost)
      {
        switch (LewdableTanks.Control.Instance.Settings.LostVehicleAction)
        {
          case PlayerVehicleAction.Salvage:
            Contract_GenerateSalvage_ProccessPlayerMech.SalvageVehicle(Contract, vehicle, mech1);
            break;
          case PlayerVehicleAction.Return:
            Contract_GenerateSalvage_ProccessPlayerMech.ReturnVehicle(Contract, vehicle, mech1);
            break;
          case PlayerVehicleAction.SalvageParts:
            Contract_GenerateSalvage_ProccessPlayerMech.SalvageParts(Contract, vehicle, mech1);
            break;
          case PlayerVehicleAction.ReturnParts:
            Contract_GenerateSalvage_ProccessPlayerMech.ReturnParts(Contract, vehicle, mech1);
            break;
        }
      }
      return false;
    }

    private static bool IsDeadVehicle(this AbstractActor vehicle)
    {
      LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Death, "Check Death for {0}, deathmethod: {1}", (object) vehicle.PilotableActorDef.Description.Id, (object) vehicle.DeathMethod);
      int num1;
      int num2;
      switch (vehicle.DeathMethod)
      {
        case DeathMethod.NOT_SET:
        case DeathMethod.DespawnedEscaped:
          num1 = 0;
          goto label_5;
        case DeathMethod.DespawnedNoMessage:
          num2 = 0;
          break;
        default:
          num2 = 1;
          break;
      }
      num1 = num2;
label_5:
      return (uint) num1 > 0U;
    }

    private static bool CanRecoverVehicle(AbstractActor vehicle)
    {
      if (!vehicle.IsDeadVehicle())
        return true;
      switch (LewdableTanks.Control.Instance.Settings.Recovery)
      {
        case PlayerVehicleRecoveryType.NoRecovery:
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, " --- allways recovery");
          return false;
        case PlayerVehicleRecoveryType.SimGameConstant:
          float num1 = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance + LewdableTanks.Control.Instance.Settings.RecoveryChanceConstantMod;
          float num2 = CustomShops.Control.State.Sim.NetworkRandom.Float();
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, {2}", (object) num1, (object) num2, (double) num2 < (double) num1 ? (object) "recovered" : (object) "failed");
          return (double) num2 < (double) num1;
        case PlayerVehicleRecoveryType.AlwaysRecovery:
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, " --- no recovery");
          return true;
        case PlayerVehicleRecoveryType.HpLeft:
          float num3 = vehicle.SummaryArmorMax + vehicle.SummaryStructureMax;
          float num4 = (vehicle.SummaryArmorCurrent + vehicle.SummaryStructureCurrent) / num3 * LewdableTanks.Control.Instance.Settings.RecoveryChanceHPMod + LewdableTanks.Control.Instance.Settings.RecoveryChanceHPBase;
          float num5 = CustomShops.Control.State.Sim.NetworkRandom.Float();
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, {2}", (object) num4, (object) num5, (double) num5 < (double) num4 ? (object) "recovered" : (object) "failed");
          return (double) num5 < (double) num4;
        case PlayerVehicleRecoveryType.HpLeftConstant:
          float num6 = vehicle.SummaryArmorMax + vehicle.SummaryStructureMax;
          float num7 = ((vehicle.SummaryArmorCurrent + vehicle.SummaryStructureCurrent) / num6 + LewdableTanks.Control.Instance.Settings.RecoveryChanceHPBase) * LewdableTanks.Control.Instance.Settings.RecoveryChanceHPMod;
          float num8 = CustomShops.Control.State.Sim.Constants.Salvage.DestroyedMechRecoveryChance + LewdableTanks.Control.Instance.Settings.RecoveryChanceConstantMod;
          float num9 = num8 + num7;
          float num10 = CustomShops.Control.State.Sim.NetworkRandom.Float();
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, " --- chance:{0:0.00} roll:{1:0.00}, base:{2:0.00}, Hp:{3:0.00} {4}", (object) num9, (object) num10, (object) num8, (object) num7, (double) num10 < (double) num9 ? (object) "recovered" : (object) "failed");
          return (double) num10 < (double) num9;
        default:
          return false;
      }
    }

    private static int NumParts(Mech vehicle, SimGameState simgame)
    {
      int min = 1;
      int defaultMechPartMax = simgame.Constants.Story.DefaultMechPartMax;
      float num1 = vehicle.SummaryArmorMax + vehicle.SummaryStructureMax;
      float num2 = vehicle.SummaryArmorCurrent + vehicle.SummaryStructureCurrent;
      int num3 = Mathf.Clamp(Mathf.CeilToInt(num2 / num1 * (float) defaultMechPartMax), min, defaultMechPartMax);
      LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- hp: {0:0.0}/{1:0.0} parts:{2}", (object) num2, (object) num1, (object) num3);
      return num3;
    }

    public static void ReturnVehicle(ContractHelper contract, Mech vehicle, MechDef mech)
    {
      string id = vehicle.MechDef.Description.Id;
      if (vehicle.MechDef == null)
        LewdableTanks.Control.Instance.LogError("No vehicledef for return");
      Contract_GenerateSalvage_ProccessPlayerMech.ReturnParts(contract, vehicle, mech);
      float moduleRecoveryChance = LewdableTanks.Control.Instance.Settings.ModuleRecoveryChance;
      foreach (MechComponentRef mechComponentRef in vehicle.MechDef.Inventory)
      {
        float num = CustomShops.Control.State.Sim.NetworkRandom.Float();
        if (mechComponentRef.DamageLevel != ComponentDamageLevel.Destroyed)
        {
          if ((double) num < (double) moduleRecoveryChance)
          {
            LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {1:0.00}<{2:0.00}, recovered {0}", (object) mechComponentRef.ComponentDefID, (object) num, (object) moduleRecoveryChance);
            contract.AddComponentToFinalSalvage(mechComponentRef.Def);
          }
          else
            LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {1:0.00}>{2:0.00}, destroyed {0}", (object) mechComponentRef.ComponentDefID, (object) num, (object) moduleRecoveryChance);
        }
        else
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {0}, DESTROYED", (object) mechComponentRef.ComponentDefID);
      }
    }

    public static void SalvageVehicle(ContractHelper contract, Mech vehicle, MechDef mech)
    {
      string id = vehicle.PilotableActorDef.Description.Id;
      if (vehicle.PilotableActorDef == null)
        LewdableTanks.Control.Instance.LogError("No vehicledef for return");
      Contract_GenerateSalvage_ProccessPlayerMech.SalvageParts(contract, vehicle, mech);
      float moduleRecoveryChance = LewdableTanks.Control.Instance.Settings.ModuleRecoveryChance;
      foreach (MechComponentRef mechComponentRef in vehicle.MechDef.Inventory)
      {
        float num = CustomShops.Control.State.Sim.NetworkRandom.Float();
        if (mechComponentRef.DamageLevel != ComponentDamageLevel.Destroyed)
        {
          if ((double) num < (double) moduleRecoveryChance)
          {
            LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {1:0.00}<{2:0.00}, recovered {0}", (object) mechComponentRef.ComponentDefID, (object) num, (object) moduleRecoveryChance);
            contract.AddComponentToPotentialSalvage(mechComponentRef.Def, mechComponentRef.DamageLevel, false);
          }
          else
            LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {1:0.00}>{2:0.00}, destroyed {0}", (object) mechComponentRef.ComponentDefID, (object) num, (object) moduleRecoveryChance);
        }
        else
          LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "-- {0}, DESTROYED", (object) mechComponentRef.ComponentDefID);
      }
    }

    private static void ReturnParts(ContractHelper contract, Mech vehicle, MechDef mech)
    {
      if (!string.IsNullOrEmpty(LewdableTanks.Control.Instance.Settings.NoVehiclePartsTag) && vehicle.MechDef.MechTags.Contains(LewdableTanks.Control.Instance.Settings.NoVehiclePartsTag))
      {
        LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "Returning {0} - no parts by tags", (object) mech.Description.Id);
      }
      else
      {
        SimGameState simulation = contract.Contract.BattleTechGame.Simulation;
        int num = Contract_GenerateSalvage_ProccessPlayerMech.NumParts(vehicle, simulation);
        if (!string.IsNullOrEmpty(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag) && vehicle.MechDef.MechTags.Contains(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag))
          return;
        contract.AddMechPartsToFinalSalvage(simulation.Constants, mech, num);
      }
    }

    private static void SalvageParts(ContractHelper contract, Mech vehicle, MechDef mech)
    {
      if (!string.IsNullOrEmpty(LewdableTanks.Control.Instance.Settings.NoVehiclePartsTag) && vehicle.MechDef.MechTags.Contains(LewdableTanks.Control.Instance.Settings.NoVehiclePartsTag))
      {
        LewdableTanks.Control.Instance.LogDebug(LewdableTanks.DInfo.Salvage, "Salvaging {0} - no parts by tags", (object) mech.Description.Id);
      }
      else
      {
        SimGameState simulation = contract.Contract.BattleTechGame.Simulation;
        int num = Contract_GenerateSalvage_ProccessPlayerMech.NumParts(vehicle, simulation);
        if (!string.IsNullOrEmpty(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag) && vehicle.MechDef.MechTags.Contains(CustomSalvage.Control.Instance.Settings.NoSalvageVehicleTag))
          return;
        contract.AddMechPartsToPotentialSalvage(simulation.Constants, mech, num);
      }
    }
  }
}
