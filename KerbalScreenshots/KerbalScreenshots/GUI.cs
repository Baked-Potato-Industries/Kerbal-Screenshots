using KSP.UI.Screens;
using KSP.UI.Screens.Mapview.MapContextMenuOptions;
using System;
using System.Threading.Tasks;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

// code taken STRAIGHT from physics range extender's source code. so kudos ig

namespace KerbalScreenshots
{
    [KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
    public class Gui : MonoBehaviour
    {
        private const float WindowWidth = 250;
        private const float DraggableHeight = 40;
        private const float LeftIndent = 12;
        private const float ContentTop = 20;
        public static Gui Fetch;
        public static bool GuiEnabled;
        private ApplicationLauncherButton button = null;
        private readonly float contentWidth = WindowWidth - 2 * LeftIndent;
        private readonly float entryHeight = 20;

        private bool _gameUiToggle;

        private float _windowHeight = 250;
        private Rect _windowRect;

        private bool awaitingInput = false;

        private void Awake()
        {
            if (Fetch)
                Destroy(Fetch);

            Fetch = this;
        }

        private void Start()
        {
            _windowRect = new Rect(Screen.width - WindowWidth - 40, 100, WindowWidth, _windowHeight);
            AddToolbarButton();
            GameEvents.onHideUI.Add(GameUiDisable);
            GameEvents.onShowUI.Add(GameUiEnable);
            _gameUiToggle = true;
        }

        private void OnDestroy()
        {
            GameEvents.onShowUI.Remove(GameUiEnable);
            GameEvents.onHideUI.Remove(GameUiDisable);
            ApplicationLauncher.Instance.RemoveModApplication(this.button);
            this.button = null;
        }

        // ReSharper disable once InconsistentNaming
        private void OnGUI()
        {
            if (!Settings.ConfigLoaded) return;
            if (GuiEnabled && _gameUiToggle)
                _windowRect = GUI.Window(320, _windowRect, GuiWindow, "");
        }

        private void GuiWindow(int windowId)
        {
            GUI.DragWindow(new Rect(0, 0, WindowWidth, DraggableHeight));
            float line = 0;


            DrawTitle();
            line++;
            DrawRebindButton(line);
            line++;
            DrawLoggingButton(line);


            _windowHeight = ContentTop + line * entryHeight + entryHeight + entryHeight;
            _windowRect.height = _windowHeight;
        }


        private void DrawRebindButton(float line)
        {
            string buttonText = "Current: " + Settings.ScreenshotKey.ToString() + " (click to set)";
            var saveRect = new Rect(LeftIndent, ContentTop + line * entryHeight, contentWidth - 2 * LeftIndent, entryHeight);
            
            if (GUI.Button(saveRect, buttonText))
            {
                awaitingInput = true;
            }

            if (awaitingInput)
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(keyCode))
                        {
                            Settings.ScreenshotKey = keyCode;
                            awaitingInput = false;
                            Apply();
                        }
                    }
                }
            }
        }
        private void DrawLoggingButton(float line)
        {
            string buttonText = Settings.loggingEnabled ? "Disable logging" : "Enable logging";
            var saveRect = new Rect(LeftIndent, ContentTop + line * entryHeight, contentWidth - 2 * LeftIndent, entryHeight);
            if (GUI.Button(saveRect, buttonText))
            {
                Settings.loggingEnabled = !Settings.loggingEnabled;
                string which = Settings.loggingEnabled ? "enabled" : "disabled";
                Debug.Log($"Kerbal Screenshots: Logging {which}");
                Settings.SaveConfig();
            }
        }

        public KeyCode Rebind()
        {
            bool newKeyFound = false;
            KeyCode newKey = KeyCode.F1;
            while (!newKeyFound)
            {
                if (Input.anyKeyDown)
                {
                    foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
                    {
                        if (Input.GetKey(keyCode))
                        {
                            newKeyFound = true;
                            newKey = keyCode;
                        }
                    }
                }
            }

            return newKey;
        }
        public void Apply()
        {
            try
            {
                KerbalScreenshotsCore.screenshotKey = Settings.ScreenshotKey;
                KerbalScreenshotsCore.Log("Kerbal Screenshots: Screenshot hotkey set to " + KerbalScreenshotsCore.screenshotKey);
            }
            catch (Exception e)
            {
                KerbalScreenshotsCore.Log($"Kerbal Screenshots: Could not save new screenshot key!: {e}");
            }

            Settings.SaveConfig();
        }
        private void DrawTitle()
        {
            var centerLabel = new GUIStyle
            {
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = Color.white }
            };
            var titleStyle = new GUIStyle(centerLabel)
            {
                fontSize = 10,
                alignment = TextAnchor.MiddleCenter
            };
            GUI.Label(new Rect(0, 0, WindowWidth, 20), "Kerbal Screenshots", titleStyle);
        }

        private void AddToolbarButton()
        {
            if (null == this.button)
            {
                Texture buttonTexture = GameDatabase.Instance.GetTexture("KerbalScreenshots/Textures/icon", false);
                this.button = ApplicationLauncher.Instance.AddModApplication(
                        EnableGui, DisableGui, Dummy, Dummy, Dummy, Dummy,
                        ApplicationLauncher.AppScenes.ALWAYS, buttonTexture
                    );
            }
        }

        private void EnableGui()
        {
            GuiEnabled = true;
            KerbalScreenshotsCore.Log("Kerbal Screenshots: Showing GUI (''Courtesy'' of lisias & Physics Range Extender!)");
        }

        private void DisableGui()
        {
            GuiEnabled = false;
            KerbalScreenshotsCore.Log("Kerbal Screenshots: Hiding GUI");
        }

        private void Dummy()
        {
        }

        private void GameUiEnable()
        {
            _gameUiToggle = true;
        }

        private void GameUiDisable()
        {
            _gameUiToggle = false;
        }
    }
}
