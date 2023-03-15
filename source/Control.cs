using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Harmony;
using HBS.Logging;
using HBS.Util;

namespace LewdableTanks
{
    public class Control
    {
        private const string ModName = "LewdableTanks";
        public Settings Settings = new Settings();

        private static Control _control;

        public static Control Instance
        {
            get
            {
                if (_control == null)
                    _control = new Control();
                return _control;
            }
        }

        public static void Init(string directory, string settingsJSON)
        {
            Instance.init(directory, settingsJSON);
        }

        private void init(string directory, string settingsJSON)
        {
            try
            {
                try
                {
                    Settings = new Settings();
                    JSONSerializationUtility.FromJSON(Settings, settingsJSON);
                }
                catch (Exception)
                {
                    Settings = new();
                }

                var harmony = HarmonyInstance.Create($"{ModName}");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Main.Info?.Log("=========================================================");
                Log.Main.Info?.Log($"Loaded {ModName} v0.5 for bt 1.9");
                Log.Main.Info?.Log("=========================================================");

                if (Settings.ShowSettingsOnLoad) Log.Main.Debug?.Log(JSONSerializationUtility.ToJSON(Settings));
                CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());

                if (Settings.FixMechPartCost)
                    CustomComponents.MechDefProcessing.Instance.Register(new VehicleCostFixer());
                if (Settings.FixUIName)
                    CustomComponents.MechDefProcessing.Instance.Register(new VehicleUINameFixer());
                if (Settings.AddWeaponToDescription)
                    CustomComponents.MechDefProcessing.Instance.Register(new VehicleDescriptionFixer());

                Log.Main.Debug?.Log("done");

            }
            catch (Exception e)
            {
                Log.Main.Error?.Log(e);
            }
        }
    }
}
