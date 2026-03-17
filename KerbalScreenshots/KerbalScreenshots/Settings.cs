using System;
using System.IO;
using UnityEngine;

// settings code ALSO taken straight from PRE's source code. thanks lisias

namespace KerbalScreenshots
{
    [KSPAddon(KSPAddon.Startup.Instantly, false)]
    public class Settings : MonoBehaviour
    {
        public static string SettingsConfigUrl = "GameData/KerbalScreenshots/settings.cfg";
        public static KeyCode ScreenshotKey { get; set; } = KeyCode.F1;
        public static bool loggingEnabled { get; set; } = true;
        public static bool ConfigLoaded { get; set; } = false;

        public static void LoadConfig()
        {
            try
            {
                KerbalScreenshotsCore.Log("Kerbal Screenshots: Loading settings.cfg...");

                ConfigNode fileNode = ConfigNode.Load(SettingsConfigUrl);
                if (!fileNode.HasNode("KerbalScreenshotsSettings"))
                {
                    KerbalScreenshotsCore.Log("Kerbal Screenshots: No `settings.cfg` file found @ " + SettingsConfigUrl);
                    return;
                }

                ConfigNode settings = fileNode.GetNode("KerbalScreenshotsSettings");

                ScreenshotKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), settings.GetValue("ScreenshotKey"));
                if (ScreenshotKey != KeyCode.None)
                {
                    KerbalScreenshotsCore.screenshotKey = ScreenshotKey;
                    KerbalScreenshotsCore.Log("Kerbal Screenshots: Screenshot hotkey set to " + KerbalScreenshotsCore.screenshotKey);
                }
                else
                {
                    KerbalScreenshotsCore.screenshotKey = KeyCode.F1;
                    KerbalScreenshotsCore.Log("Kerbal Screenshots: Screenshot hotkey set to default (F1)");
                }

                loggingEnabled = bool.Parse(settings.GetValue("loggingEnabled"));
            }
            catch (Exception ex)
            {
                KerbalScreenshotsCore.Log("Kerbal Screenshots: Failed to load settings config:" + ex.Message);
            }
        }

        public static void SaveConfig()
        {
            try
            {
                ConfigNode fileNode = ConfigNode.Load(SettingsConfigUrl);
                if (!fileNode.HasNode("KerbalScreenshotsSettings"))
                {
                    KerbalScreenshotsCore.Log("Kerbal Screenshots: No `settings.cfg` file found @ " + SettingsConfigUrl);
                    return;
                }
                ConfigNode settings = fileNode.GetNode("KerbalScreenshotsSettings");

                settings.SetValue("ScreenshotKey", ScreenshotKey.ToString());
                settings.SetValue("LoggingEnabled", Settings.loggingEnabled);

                KerbalScreenshotsCore.Log("Kerbal Screenshots: `settings.cfg` saved!");
                fileNode.Save(SettingsConfigUrl);

            }
            catch (Exception ex)
            {
                KerbalScreenshotsCore.Log("Kerbal Screenshots: Failed to save settings config:" + ex.Message); throw;
            }
        }


    }
}
