using UnityEngine;

namespace JudahsSpeedUtils
{
    public class ConfigUi : MonoBehaviour
    {
        public bool open = true;

        private Rect winRect = new(20, 20, 275, 695);

        public void OnGUI()
        {
            if (open)
            {
                winRect = GUI.Window(0, winRect, WinProc, $"{PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION})");
            }
        }

        private float highestSpeed = 0f;

        private void WinProc(int id)
        {
            var ox = 15f;
            var oy = 30f;
            var smallButtonHeight = 20f;
            
            var mx = winRect.width - 30;

            {
                var spd = Tools.Instance.GetPlayerSpeed();

                if (spd > highestSpeed) highestSpeed = spd;

                GUI.Label(new(ox, oy, mx, 20), $"Speed: {spd:F}");
                oy += 10 + 5;

                GUI.Label(new(ox, oy, mx, 20), $"Highest: {highestSpeed:F}");
                oy += 10 + 5;
            }

            oy += 0f;

            {
                var fps = Tools.Instance.FpsLimit;
                var limiting = Tools.Instance.LimitFramerate;

                GUI.Label(new(ox, oy, mx, 20), $"FPS: {1 / Time.deltaTime:F0}");
                oy += 10 + 5;

                GUI.Label(new(ox, oy, mx, 20), $"FPS Limit: {(limiting ? "<color=green>On</color>" : "<color=red>Off</color>")} ({fps})");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, (mx / 2) - 5, 20), $"Toggle Limit (L)"))
                {
                    Tools.Instance.ToggleFpsLimiter();
                }

                if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, 20), "Set FPS (30-60)"))
                {
                    if (fps == 30)
                    {
                        Tools.Instance.FpsLimit = 60;
                    }
                    else
                    {
                        Tools.Instance.FpsLimit = 30;
                    }

                    if (limiting)
                    {
                        Application.targetFrameRate = Tools.Instance.FpsLimit;
                    }
                }

                oy += 20 + 5;
            }

            oy += 0f;

            {
                GUI.Label(new(ox, oy, mx, 20), $"Rep: {Tools.Instance.GetCurrentRep()}/{Tools.Instance.GetRequiredRep()}");
                oy += 10 + 5;

                GUI.Label(new(ox, oy, mx, 20), $"Current Stage: {Tools.Instance.GetCurrentStage()}");
                oy += 10 + 5;

                GUI.Label(new(ox, oy, mx, 20), $"Selected Stage: {Tools.Instance.SelectedStage}");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, (mx / 2) - 5, smallButtonHeight), $"Go To Stage"))
                {
                    Tools.Instance.SwitchToSelectedStage();
                }
                if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, smallButtonHeight), $"Select Next Stage"))
                {
                    Tools.Instance.SelectNextStage();
                }

                oy += 20 + 5;
            }

            {
                var wanted = Tools.Instance.PlayerIsWanted();
                GUI.Label(new(ox, oy, mx, 20), $"Wanted: {(wanted ? "<color=green>Yes</color>" : "<color=red>No</color>")}");

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "End Wanted Status (K)"))
                {
                    Tools.Instance.StopWantedStatus();
                }

                oy += 20 + 5;

                var inv = Tools.Instance.Invulnerable;
                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), $"Invulnerable (I): {(inv ? "<color=green>On</color>" : "<color=red>Off</color>")}"))
                {
                    Tools.Instance.Invulnerable = !Tools.Instance.Invulnerable;
                }

                oy += 20 + 5;
            }

            {
                var loc = Tools.Instance.SavedPlayerLocation;
                GUI.Label(new(ox, oy, mx, 20), $"Saved: ({loc.position.x}, {loc.position.y}, {loc.position.z})");

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Save Player Position (H)"))
                {
                    Tools.Instance.SavePlayerPosition();
                }

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Teleport to Position (J)"))
                {
                    Tools.Instance.ResetPlayerPosition();
                }

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Refil Boost (R)"))
                {
                    Tools.Instance.RefilPlayerBoost();
                }

                oy += 20 + 5;
            }

            oy += 10;

            {
                GUI.Label(new(ox, oy, mx, 20), $"Character: {Tools.Instance.CurrentChar} ({Tools.Instance.OutfitIndex})");
                oy += 10 + 5;

                GUI.Label(new(ox, oy, mx, 20), $"Move Style: {Tools.Instance.CurrentStyle}");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Play As Next Character"))
                {
                    Tools.Instance.PlayAsNextCharacter();
                }

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Use Next Move Style"))
                {
                    Tools.Instance.UseNextMoveStyle();
                }

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), "Wear Next Outfit"))
                {
                    Tools.Instance.WearNextOutfit();
                }

                oy += 20 + 5;
            }

            oy += 0f;

            {
                var TimeScaleMod = Tools.Instance.TimeMod;

                GUI.Label(new(ox, oy, mx, 20), $"TimeScale: {TimeScaleMod:F}");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, (mx / 2) - 5, smallButtonHeight), $"TimeScale [-] (9)"))
                {
                    Tools.Instance.DecreaseTimeScale();
                }
                if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, smallButtonHeight), $"TimeScale [+] (0)"))
                {
                    Tools.Instance.IncreaseTimeScale();
                }

                oy += 20 + 5;
            }

            oy += 0f;

            {
                var MoveMod = Tools.Instance.FlightSpeed;
                var freezeHeight = Tools.Instance.FreezeHeight;

                GUI.Label(new(ox, oy, mx, 20), $"Fly Speed: {MoveMod:F}");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, mx, smallButtonHeight), $"Enable Flight (\\): {(freezeHeight ? "<color=green>On</color>" : "<color=red>Off</color>")}"))
                {
                    Tools.Instance.FreezeHeight = !Tools.Instance.FreezeHeight;
                }

                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, (mx / 2) - 5, smallButtonHeight), $"Fly Speed [-] (-)"))
                {
                    Tools.Instance.DecreaseFlySpeed();
                }
                if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, smallButtonHeight), $"Fly Speed [+] (=)"))
                {
                    Tools.Instance.IncreaseFlySpeed();
                }

                oy += 20 + 5;
            }

            oy += 0f;

            {
                var flyHeight = Tools.Instance.FlightHeight;

                GUI.Label(new(ox, oy, mx, 20), $"FlyHeight: {flyHeight:F}");
                oy += 20 + 5;

                if (GUI.Button(new(ox, oy, (mx / 2) - 5, smallButtonHeight), $"Fly Height [-] ([)"))
                {
                    Tools.Instance.DecreaseFlyHeight();
                }
                if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, smallButtonHeight), $"Fly Height [+] (])"))
                {
                    Tools.Instance.IncreaseFlyHeight();
                }

                oy += 20 + 5;
            }

            GUI.DragWindow();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Quote)) open = !open;
            if (Input.GetKeyDown(KeyCode.L)) Tools.Instance.ToggleFpsLimiter();
            if (Input.GetKeyDown(KeyCode.K)) Tools.Instance.StopWantedStatus();
            if (Input.GetKeyDown(KeyCode.I)) Tools.Instance.Invulnerable = !Tools.Instance.Invulnerable;
            if (Input.GetKeyDown(KeyCode.H)) Tools.Instance.SavePlayerPosition();
            if (Input.GetKeyDown(KeyCode.J)) Tools.Instance.ResetPlayerPosition();
            if (Input.GetKeyDown(KeyCode.R)) Tools.Instance.RefilPlayerBoost();
            if (Input.GetKey(KeyCode.LeftBracket)) Tools.Instance.DecreaseFlyHeight();
            if (Input.GetKey(KeyCode.RightBracket)) Tools.Instance.IncreaseFlyHeight();
            if (Input.GetKey(KeyCode.Minus)) Tools.Instance.DecreaseFlySpeed();
            if (Input.GetKey(KeyCode.Equals)) Tools.Instance.IncreaseFlySpeed();
            if (Input.GetKeyDown(KeyCode.Backslash)) Tools.Instance.FreezeHeight = !Tools.Instance.FreezeHeight;
            if (Input.GetKeyDown(KeyCode.Alpha9)) Tools.Instance.DecreaseTimeScale();
            if (Input.GetKeyDown(KeyCode.Alpha0)) Tools.Instance.IncreaseTimeScale();
        }
    }
}