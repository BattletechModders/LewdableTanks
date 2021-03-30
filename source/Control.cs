using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using BattleTech;
using CustomSalvage;
using Harmony;
using HBS.Logging;
using HBS.Util;
using UnityEngine;

namespace LewdableTanks
{
    public class Control
    {
        private const string ModName = "LewdableTanks";
        private string LogPrefix = "[LTMod]";
        public Settings Settings = new Settings();
        private static ILog Logger;
        private static FileLogAppender logAppender;

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

                Logger = HBS.Logging.Logger.GetLogger(ModName, LogLevel.Debug);
                try
                {
                    Settings = new Settings();
                    JSONSerializationUtility.FromJSON(Settings, settingsJSON);
                    HBS.Logging.Logger.SetLoggerLevel(Logger.Name, LogLevel.Debug);
                }
                catch (Exception)
                {
                    Settings = new Settings();
                }

                if (!Settings.AddLogPrefix)
                    LogPrefix = "";

                SetupLogging(directory);

                var harmony = HarmonyInstance.Create($"{ModName}");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Logger.Log("=========================================================");
                Logger.Log($"Loaded {ModName} v0.5 for bt 1.9");
                Logger.Log("=========================================================");

                if (Settings.ShowSettingsOnLoad) Logger.LogDebug(JSONSerializationUtility.ToJSON(Settings));
                CustomComponents.Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());

                if (Settings.FixMechPartCost)
                    CustomComponents.AutoFixer.Shared.RegisterMechFixer(Extentions.FixVehicleCost, Settings.FakeVehicleTag);
                if (Settings.FixUIName)
                    CustomComponents.AutoFixer.Shared.RegisterMechFixer(Extentions.FixVehicleUIName, Settings.FakeVehicleTag);
                if (Settings.AddWeaponToDescription)
                    CustomComponents.AutoFixer.Shared.RegisterMechFixer(Extentions.FixDescription, Settings.FakeVehicleTag);

                Logger.LogDebug("done");

            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }

        #region LOGGING

        [Conditional("CCDEBUG")]
        public void LogDebug(DInfo type, string message, params object[] list)
        {
            if (Settings.DebugInfo.HasFlag(type))
                Logger.LogDebug(LogPrefix + string.Format(message, list));
        }
        [Conditional("CCDEBUG")]
        public void LogDebug(DInfo type, Exception e, string message, params object[] list)
        {
            if (Settings.DebugInfo.HasFlag(type))
                Logger.LogDebug(LogPrefix + string.Format(message, list), e);
        }

        public void LogError(string message)
        {
            Logger.LogError(LogPrefix + message);
        }
        public void LogError(string message, Exception e)
        {
            Logger.LogError(LogPrefix + message, e);
        }
        public void LogError(Exception e)
        {
            Logger.LogError(LogPrefix, e);
        }

        public void Log(string message)
        {
            Logger.Log(LogPrefix + message);
        }



        internal void SetupLogging(string Directory)
        {
            var logFilePath = Path.Combine(Directory, "log.txt");

            try
            {
                ShutdownLogging();
                AddLogFileForLogger(logFilePath);
            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        internal void ShutdownLogging()
        {
            if (logAppender == null)
            {
                return;
            }

            try
            {
                HBS.Logging.Logger.ClearAppender(ModName);
                logAppender.Flush();
                logAppender.Close();
            }
            catch
            {
            }

            logAppender = null;
        }

        private static void AddLogFileForLogger(string logFilePath)
        {
            try
            {
                logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);
                HBS.Logging.Logger.AddAppender(ModName, logAppender);

            }
            catch (Exception e)
            {
                Logger.Log($"{ModName}: can't create log file", e);
            }
        }

        #endregion

    }
}
