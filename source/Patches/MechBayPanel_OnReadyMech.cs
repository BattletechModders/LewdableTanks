using BattleTech;
using BattleTech.UI;
using CustomSalvage;
using CustomUnits;
using Harmony;
using Localize;
using System;

namespace LewdableTanks.Patches
{
  [HarmonyPatch(typeof (MechBayPanel))]
  [HarmonyPatch("OnReadyMech")]
  public static class MechBayPanel_OnReadyMech
  {
    [HarmonyPrefix]
    public static bool ReadyVehicle(
      MechBayPanel __instance,
      MechBayChassisUnitElement chassisElement,
      ref MechBayChassisUnitElement ___selectedChassis,
      MechBayRowGroupWidget ___bayGroupWidget)
    {
      try
      {
        ChassisDef chassisDef = chassisElement.ChassisDef;
        if (!chassisDef.IsVehicle())
          return true;
        string id = chassisElement.ChassisDef.Description.Id;
        SimGameState sim = __instance.Sim;
        int num1 = sim.VehicleShift();
        int num2 = num1 + sim.GetMaxActiveMechs();
        int num3 = -1;
        for (int key = num1; key < num2; ++key)
        {
          MechDef mechDef = (MechDef) null;
          if (!sim.ActiveMechs.TryGetValue(key, out mechDef))
            sim.ReadyingMechs.TryGetValue(key, out mechDef);
          if (mechDef == null)
          {
            num3 = key;
            break;
          }
        }
        if (num3 < 0)
        {
          Control.Instance.LogDebug(DInfo.General, "No Free vehicle slots for {0}", (object) id);
          return false;
        }
        string mdefFromCdef = ChassisHandler.GetMDefFromCDef(id);
        string simGameUid = sim.GenerateSimGameUID();
        MechDef stockMech = __instance.DataManager.MechDefs.Get(mdefFromCdef);
        MechDef mech = new MechDef(chassisDef, simGameUid, stockMech);
        mech.SetInventory(stockMech.Inventory);
        WorkOrderEntry_ReadyMech orderEntryReadyMech = new WorkOrderEntry_ReadyMech(string.Format("ReadyMech-{0}", (object) simGameUid), Localize.Strings.T("Readying 'Mech - {0}", (object) chassisDef.Description.Name), sim.Constants.Story.MechReadyTime, num3, mech, Localize.Strings.T(sim.Constants.Story.MechReadiedWorkOrderCompletedText, (object) chassisDef.Description.Name));
        sim.MechLabQueue.Add((WorkOrderEntry) orderEntryReadyMech);
        sim.ReadyingMechs[num3] = mech;
        sim.RoomManager.AddWorkQueueEntry((WorkOrderEntry) orderEntryReadyMech);
        sim.UpdateMechLabWorkQueue(false);
        sim.RemoveItemStat(id, typeof (MechDef), false);
        AudioEventManager.PlayAudioEvent("audioeventdef_simgame_vo_barks", "workqueue_readymech", WwiseManager.GlobalAudioObject);
        ___selectedChassis = (MechBayChassisUnitElement) null;
        __instance.RefreshData();
        __instance.ViewBays();
        __instance.SelectMech(___bayGroupWidget.GetMechUnitForSlot(num3), true);
      }
      catch (Exception ex)
      {
        Control.Instance.LogError(ex);
      }
      return false;
    }
  }
}
