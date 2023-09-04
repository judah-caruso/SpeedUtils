using BepInEx;
using UnityEngine;

namespace JudahsSpeedUtils
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private GameObject _mod;
        private Tools _tools;
        private TriggerTools _triggerTools;

        private void Awake()
        {
            _tools = new();
            _triggerTools = new();

            _mod = new();
            _mod.AddComponent<ConfigUi>();
            _mod.AddComponent<Tools>();
            _mod.AddComponent<TriggerTools>();
            GameObject.DontDestroyOnLoad(_mod);
        }
    }
}

