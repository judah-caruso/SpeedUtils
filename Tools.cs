using System.Reflection;
using Reptile;
using Reptile.Phone;
using UnityEngine;

namespace JudahsSpeedUtils;

public class Tools : MonoBehaviour
{
    public static Tools Instance;

    public bool Invulnerable;
    public bool LimitFramerate = false;
    public int FpsLimit = 30;
    
    public int OutfitIndex = 0;
    public MoveStyle CurrentStyle = MoveStyle.ON_FOOT;
    public Characters CurrentChar = Characters.NONE;
    public SafeLocation SavedPlayerLocation;
    public Stage SelectedStage;
    
    private Core core;
    private WorldHandler world;
    private WantedManager wantedManager;
    private bool coreHasBeenSetup;
    
    public Tools()
    {
        Instance = this;
    }

    private bool delegateHasBeenSetup = false;
    
    public void Update()
    {
        if (!coreHasBeenSetup)
        {
            core = Core.Instance;
            if (core != null)
            {
                world = WorldHandler.instance;
                coreHasBeenSetup = world != null;
                
                if (!delegateHasBeenSetup)
                {
                    StageManager.OnStageInitialized += () =>
                    {
                        Debug.Log("Swapped to new stage!");
                        coreHasBeenSetup = false;
                    };
                    
                    delegateHasBeenSetup = true;
                }
            }
        }
        
        // Null when the player isn't wanted
        wantedManager = WantedManager.instance;

        if (coreHasBeenSetup)
        {
            var player = world.GetCurrentPlayer();
            if (Invulnerable)
            {
                player.ResetHP();
                if (player.AmountOfCuffs() > 0)
                {
                    player.RemoveAllCuffs();
                }
            }
        }
    }
    
    public void SwitchToSelectedStage()
    {
        if (!coreHasBeenSetup) return;
        if (!core.BaseModule.IsPlayingInStage) return;
        core.BaseModule.SwitchStage(SelectedStage);
    }

    private int stageIndex = 0;
    private Stage[] playableStages = new[]
    {
        Stage.city,
        Stage.downhill,
        Stage.hideout, 
        Stage.Mall, 
        Stage.osaka, 
        Stage.Prelude, 
        Stage.pyramid, 
        Stage.square, 
        Stage.tower, 
    };
    
    public void SelectNextStage()
    {
        stageIndex = (stageIndex + 1) % playableStages.Length;
        SelectedStage = playableStages[stageIndex];
    }

    public void ToggleFpsLimiter()
    {
        LimitFramerate = !LimitFramerate;
        if (LimitFramerate)
        {
            Application.targetFrameRate = FpsLimit;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }

    public int GetRequiredRep()
    {
        if (!coreHasBeenSetup) return 0;
        
        var t = world.GetType();
        var field = t.GetField("requiredREP", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
        var rep = (int)(field?.GetValue(world) ?? 0);
        if (rep == 0)
        {
            field = t.GetField("totalREPInCurrentStage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            rep = (int)(field?.GetValue(world) ?? 0);
        }

        return rep;
    }

    public int GetCurrentRep()
    {
        if (!coreHasBeenSetup) return 0;

        var save = core.SaveManager.CurrentSaveSlot;
        if (save == null) return 0;

        var progress = save.GetCurrentStageProgress();
        if (progress == null) return 0;
        
        return progress.reputation;
    }
    
    public float GetPlayerSpeed()
    {
        if (!coreHasBeenSetup) return 0f;
        var player = world.GetCurrentPlayer();
        return player.GetTotalSpeed();
    }

    public Stage GetCurrentStage()
    {
        if (!coreHasBeenSetup) return Stage.NONE;
        return core.BaseModule.CurrentStage;
    }

    public void PlayAsNextCharacter()
    {
        if (!coreHasBeenSetup) return;
        
        var player = world.GetCurrentPlayer();
        var playerType = player.GetType();
        
        if (CurrentChar == Characters.NONE)
        {
            var field = playerType.GetField("character",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            CurrentChar = (Characters)(field?.GetValue(player) ?? 0);
        }
        
        CurrentChar = (Characters)(((int)CurrentChar + 1) % (int)Characters.MAX);
        player.SetCharacter(CurrentChar);

        var initHitboxes = playerType.GetMethod("InitHitboxes", BindingFlags.NonPublic | BindingFlags.Instance);
        initHitboxes?.Invoke(player, new object[]{});

        var initCuffs = playerType.GetMethod("initCuffs", BindingFlags.NonPublic | BindingFlags.Instance);
        initCuffs?.Invoke(player, new object[]{});
    }

    public void WearNextOutfit()
    {
        if (!coreHasBeenSetup) return;
        
        var player = world.GetCurrentPlayer();
        OutfitIndex = (OutfitIndex + 1) % 4;
        player.SetOutfit(OutfitIndex);
    }
    
    public void UseNextMoveStyle()
    {
        if (!coreHasBeenSetup) return;

        var player = world.GetCurrentPlayer();
        if (CurrentStyle == MoveStyle.ON_FOOT)
        {
            var field = player.GetType().GetField("moveStyle",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            CurrentStyle = (MoveStyle)(field?.GetValue(player) ?? 0);
        }

        CurrentStyle = (MoveStyle)(((int)CurrentStyle + 1) % (int)MoveStyle.MAX);
        player.SetCurrentMoveStyleEquipped(CurrentStyle, true, true);
    }

    public bool PlayerIsWanted()
    {
        if (wantedManager != null) return wantedManager.Wanted;
        return false;
    }
    
    public void StopWantedStatus()
    {
        if (PlayerIsWanted())
        {
            Debug.Log("Stopping wanted status...");
            wantedManager.StopPlayerWantedStatus(true);
        }
    }
    
    public void SavePlayerPosition()
    {
        if (!coreHasBeenSetup) return;
        
        var player = world.GetCurrentPlayer();
        var pos = player.tf.position;
        var rot = player.tf.rotation;
        
        Debug.Log($"Saving position {pos.x}, {pos.y}, {pos.z}");
        
        SavedPlayerLocation = new()
        {
            position = pos,
            rotation = rot,
            timeStamp = Time.time,
            set = true
        };
    }

    public void ResetPlayerPosition()
    {
        if (!coreHasBeenSetup) return;
        
        var pos = SavedPlayerLocation.position;
        var rot = SavedPlayerLocation.rotation;
        
        Debug.Log($"Teleporting to {pos.x}, {pos.y}, {pos.z}");
        world.PlaceCurrentPlayerAt(pos, rot, true);
    } 
}