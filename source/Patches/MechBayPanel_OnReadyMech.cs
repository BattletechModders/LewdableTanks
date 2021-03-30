using System;
using BattleTech;
using BattleTech.UI;
using CustomSalvage;
using CustomUnits;
using Harmony;
using Localize;

namespace LewdableTanks.Patches
{
    [HarmonyPatch(typeof(MechBayPanel))]
    [HarmonyPatch("OnReadyMech")]
    public static class MechBayPanel_OnReadyMech
    {
        [HarmonyPrefix]
        public static bool ReadyVehicle(MechBayPanel __instance, MechBayChassisUnitElement chassisElement,
            ref MechBayChassisUnitElement ___selectedChassis, MechBayRowGroupWidget ___bayGroupWidget)
        {
            try
            {
                var chassisDef = chassisElement.ChassisDef;
                if (!chassisDef.IsVehicle())
                    return true;

                var id = chassisElement.ChassisDef.Description.Id;
                var sim = __instance.Sim;
                int start = sim.VehicleShift();
                int end = start + sim.GetMaxActiveMechs();
                int baySlot = -1;
                for (int i = start; i < end; i++)
                {


                    MechDef mech_in_slot = null;
                    if (!sim.ActiveMechs.TryGetValue(i, out mech_in_slot))
                        sim.ReadyingMechs.TryGetValue(i, out mech_in_slot);
                    if (mech_in_slot == null)
                    {
                        baySlot = i;
                        break;
                    }
                }

                if (baySlot < 0)
                {
                    Control.Instance.LogDebug(DInfo.General, "No Free vehicle slots for {0}", id);
                    return false;
                }

                var mid = ChassisHandler.GetMDefFromCDef(id);
                var sim_id = sim.GenerateSimGameUID();
                var stock = __instance.DataManager.MechDefs.Get(mid);
                var mech = new MechDef(chassisDef, sim_id, stock);
                mech.SetInventory(stock.Inventory);
                
                
                WorkOrderEntry_ReadyMech workOrderEntry_ReadyMech = new WorkOrderEntry_ReadyMech(
                    string.Format("ReadyMech-{0}", sim_id), Strings.T("Readying 'Mech - {0}", new object[]
                    {
                        chassisDef.Description.Name
                    }), sim.Constants.Story.MechReadyTime, baySlot, mech, Strings.T(
                        sim.Constants.Story.MechReadiedWorkOrderCompletedText, new object[]
                        {
                            chassisDef.Description.Name
                        }));

                sim.MechLabQueue.Add(workOrderEntry_ReadyMech);
                sim.ReadyingMechs[baySlot] = mech;
                sim.RoomManager.AddWorkQueueEntry(workOrderEntry_ReadyMech);
                sim.UpdateMechLabWorkQueue(false);
                sim.RemoveItemStat(id, typeof(MechDef), false);

                AudioEventManager.PlayAudioEvent("audioeventdef_simgame_vo_barks", "workqueue_readymech",
                    WwiseManager.GlobalAudioObject, null);
                
                
                ___selectedChassis = null;
                __instance.RefreshData(true);
                __instance.ViewBays();
                __instance.SelectMech(___bayGroupWidget.GetMechUnitForSlot(baySlot), true);


            }
            catch (Exception e)
            {
                Control.Instance.LogError(e);
            }

            return false;
        }
    }
}