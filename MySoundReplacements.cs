using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class MySoundReplacements : BaseUnityPlugin
{
    public static MySoundReplacements Instance { get; private set; } = null!;
    internal static new ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    internal ConfigEntry<bool> loadIntoRAM = null!;
    public bool LoadIntoRAM => loadIntoRAM.Value;

    private const string FALL_DEATH = "fall.ogg";
    internal AudioClip? fallDeath;

    private const string FREDDY_FAZBEAR = "music box.wav";
    internal AudioClip? freddyFazbear;

    private void Awake()
    {
        Logger = base.Logger;
        Instance = this;

        loadIntoRAM = Config.Bind(
            "General",
            "LoadIntoRAM",
            true,
            "Loads the sounds into RAM instead of streaming from disk"
        );

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger.LogDebug("Patching...");
        Harmony.PatchAll();
        Logger.LogDebug("Finished patching!");

        Logger.LogDebug("Loading sounds...");
        fallDeath = AudioManager.LoadSound(rel(FALL_DEATH));
        freddyFazbear = AudioManager.LoadSound(rel(FREDDY_FAZBEAR));
        Logger.LogDebug("Finished loading sounds!");

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    private string rel(string path) =>
        Path.Combine(
            Path.GetDirectoryName(GetType().Assembly.Location) ?? string.Empty,
            "Sounds",
            path
        );
}
