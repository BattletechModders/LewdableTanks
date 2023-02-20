using CustomComponents;
using Harmony;
using HBS.Logging;
using HBS.Util;
using System;
using System.IO;
using System.Reflection;

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
        if (Control._control == null)
          Control._control = new Control();
        return Control._control;
      }
    }

    public static void Init(string directory, string settingsJSON) => Control.Instance.init(directory, settingsJSON);

    private void init(string directory, string settingsJSON)
    {
      try
      {
        Control.Logger = HBS.Logging.Logger.GetLogger("LewdableTanks", HBS.Logging.LogLevel.Debug);
        try
        {
          this.Settings = new Settings();
          JSONSerializationUtility.FromJSON<Settings>(this.Settings, settingsJSON);
          HBS.Logging.Logger.SetLoggerLevel(Control.Logger.Name, new HBS.Logging.LogLevel?(HBS.Logging.LogLevel.Debug));
        }
        catch (Exception)
        {
          this.Settings = new Settings();
        }
        if (!this.Settings.AddLogPrefix)
          this.LogPrefix = "";
        this.SetupLogging(directory);
        HarmonyInstance.Create("LewdableTanks").PatchAll(Assembly.GetExecutingAssembly());
        Control.Logger.Log((object) "=========================================================");
        Control.Logger.Log((object) "Loaded LewdableTanks v0.5 for bt 1.9");
        Control.Logger.Log((object) "=========================================================");
        if (this.Settings.ShowSettingsOnLoad)
          Control.Logger.LogDebug((object) JSONSerializationUtility.ToJSON<Settings>(this.Settings));
        Registry.RegisterSimpleCustomComponents(Assembly.GetExecutingAssembly());
        if (this.Settings.FixMechPartCost)
          MechDefProcessing.Instance.Register((IMechDefProcessor)new VehicleCostFixer());
        if (this.Settings.FixUIName)
          MechDefProcessing.Instance.Register((IMechDefProcessor)new VehicleUINameFixer());
        if (this.Settings.AddWeaponToDescription)
          MechDefProcessing.Instance.Register((IMechDefProcessor)new VehicleDescriptionFixer());
        Control.Logger.LogDebug((object) "done");
      }
      catch (Exception ex)
      {
        Control.Logger.LogError((object) ex);
      }
    }

    public void LogDebug(DInfo type, string message, params object[] list)
    {
      if (!this.Settings.DebugInfo.HasFlag((Enum) type))
        return;
      Control.Logger.LogDebug((object) (this.LogPrefix + string.Format(message, list)));
    }

    public void LogDebug(DInfo type, Exception e, string message, params object[] list)
    {
      if (!this.Settings.DebugInfo.HasFlag((Enum) type))
        return;
      Control.Logger.LogDebug((object) (this.LogPrefix + string.Format(message, list)), e);
    }

    public void LogError(string message) => Control.Logger.LogError((object) (this.LogPrefix + message));

    public void LogError(string message, Exception e) => Control.Logger.LogError((object) (this.LogPrefix + message), e);

    public void LogError(Exception e) => Control.Logger.LogError((object) this.LogPrefix, e);

    public void Log(string message) => Control.Logger.Log((object) (this.LogPrefix + message));

    internal void SetupLogging(string Directory)
    {
      string logFilePath = Path.Combine(Directory, "log.txt");
      try
      {
        this.ShutdownLogging();
        Control.AddLogFileForLogger(logFilePath);
      }
      catch (Exception ex)
      {
        Control.Logger.Log((object) "LewdableTanks: can't create log file", ex);
      }
    }

    internal void ShutdownLogging()
    {
      if (Control.logAppender == null)
        return;
      try
      {
        HBS.Logging.Logger.ClearAppender("LewdableTanks");
        Control.logAppender.Flush();
        Control.logAppender.Close();
      }
      catch
      {
      }
      Control.logAppender = (FileLogAppender) null;
    }

    private static void AddLogFileForLogger(string logFilePath)
    {
      try
      {
        Control.logAppender = new FileLogAppender(logFilePath, FileLogAppender.WriteMode.INSTANT);
        HBS.Logging.Logger.AddAppender("LewdableTanks", (ILogAppender) Control.logAppender);
      }
      catch (Exception ex)
      {
        Control.Logger.Log((object) "LewdableTanks: can't create log file", ex);
      }
    }
  }
}
