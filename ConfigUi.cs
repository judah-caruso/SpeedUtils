using UnityEngine;

namespace JudahsSpeedUtils;

public class ConfigUi : MonoBehaviour
{
    public bool open = true;

    private Rect winRect = new(20, 20, 275, 600);

    public void OnGUI()
    {
        if (open)
        {
            winRect = GUI.Window(0, winRect, WinProc, $"{PluginInfo.PLUGIN_NAME} ({PluginInfo.PLUGIN_VERSION})");
        }
    }

    private void WinProc(int id)
    {
        var ox = 15f;
        var oy = 30f;
        var mx = winRect.width - 30;
        
        {
            var fps = Tools.Instance.FpsLimit;
            var limiting = Tools.Instance.LimitFramerate;
            
            GUI.Label(new(ox, oy, mx, 20), $"FPS: {1 / Time.deltaTime:F0}");
            oy += 20 + 5;
            
            GUI.Label(new(ox, oy, mx, 20), $"FPS Limit: {(limiting ? "<color=green>On</color>" : "<color=red>Off</color>")} ({fps})");
            oy += 20 + 5;

            if (GUI.Button(new(ox, oy, (mx / 2) - 5, 30), $"Toggle Limit (L)"))
            {
                Tools.Instance.ToggleFpsLimiter();
            }
            
            if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, 30), "Set FPS (30-60)"))
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

            oy += 30 + 5;
        }

        oy += 10;
        
        {
            GUI.Label(new(ox, oy, mx, 20), $"Rep: {Tools.Instance.GetCurrentRep()}/{Tools.Instance.GetRequiredRep()}");
            oy += 20 + 5;
            
            GUI.Label(new(ox, oy, mx, 20), $"Current Stage: {Tools.Instance.GetCurrentStage()}");
            oy += 20 + 5;
            
            GUI.Label(new(ox, oy, mx, 20), $"Selected Stage: {Tools.Instance.SelectedStage}");
            oy += 20 + 5;
            
            if (GUI.Button(new(ox, oy, (mx / 2) - 5, 30), $"Go To Stage"))
            {
                Tools.Instance.SwitchToSelectedStage();
            }
            if (GUI.Button(new(ox + 5 + (mx / 2), oy, (mx / 2) - 5, 30), $"Select Next Stage"))
            {
                Tools.Instance.SelectNextStage();
            }

            oy += 30 + 5;
        }

        {
            var wanted = Tools.Instance.PlayerIsWanted();
            GUI.Label(new(ox, oy, mx, 20), $"Wanted: {(wanted ? "<color=green>Yes</color>" : "<color=red>No</color>")}");

            oy += 20 + 5;
            
            if (GUI.Button(new(ox, oy, mx, 30), "End Wanted Status (K)"))
            {
                Tools.Instance.StopWantedStatus();
            }

            oy += 30 + 5;

            var inv = Tools.Instance.Invulnerable;
            if (GUI.Button(new(ox, oy, mx, 30), $"Invulnerable (I): {(inv ? "<color=green>On</color>" : "<color=red>Off</color>")}"))
            {
                Tools.Instance.Invulnerable = !Tools.Instance.Invulnerable;
            }

            oy += 30 + 5;
        }

        {
            var loc = Tools.Instance.SavedPlayerLocation;
            GUI.Label(new(ox, oy, mx, 20), $"Saved: ({loc.position.x}, {loc.position.y}, {loc.position.z})");

            oy += 20 + 5;

            if (GUI.Button(new(ox, oy, mx, 30), "Save Player Position (H)"))
            {
                Tools.Instance.SavePlayerPosition();
            }

            oy += 30 + 5;
            
            if (GUI.Button(new(ox, oy, mx, 30), "Teleport to Position (J)"))
            {
                Tools.Instance.ResetPlayerPosition();
            }

            oy += 30 + 5;
        }

        oy += 10;
        
        {
            GUI.Label(new(ox, oy, mx, 20), $"Character: {Tools.Instance.CurrentChar} ({Tools.Instance.OutfitIndex})");
            oy += 20 + 5;
            
            GUI.Label(new(ox, oy, mx, 20), $"Move Style: {Tools.Instance.CurrentStyle}");
            oy += 20 + 5;
            
            if (GUI.Button(new(ox, oy, mx, 30), "Play As Next Character"))
            {
                Tools.Instance.PlayAsNextCharacter();
            }

            oy += 30 + 5;
            
            if (GUI.Button(new(ox, oy, mx, 30), "Use Next Move Style"))
            {
                Tools.Instance.UseNextMoveStyle();
            }
            
            oy += 30 + 5;
            
            if (GUI.Button(new (ox, oy, mx, 30), "Wear Next Outfit"))
            {
                Tools.Instance.WearNextOutfit();
            }
            
            oy += 30 + 5;
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
    }
}