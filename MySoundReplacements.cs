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

    private ConfigEntry<bool>? fallDeathEnable;
    private AudioClip? fallDeath;

    private ConfigEntry<bool>? freddyFazbearEnable;
    private AudioClip? freddyFazbear;

    private ConfigEntry<bool>? eyeScreamEnable;
    private AudioClip? eyeScream;

    public static class Sounds
    {
        internal static bool FallDeathEnable => Instance is { fallDeathEnable.Value: true };
        internal const string FALL_DEATH = "fall.ogg";
        public static AudioClip? FallDeath => FallDeathEnable ? Instance.fallDeath : null;

        internal static bool FreddyFazbearEnable => Instance is { freddyFazbearEnable.Value: true };
        internal const string FREDDY_FAZBEAR = "music box.wav";
        public static AudioClip? FreddyFazbear =>
            FreddyFazbearEnable ? Instance.freddyFazbear : null;

        internal static bool EyeScreamEnable => Instance is { eyeScreamEnable.Value: true };
        internal const string EYE_SCREAM = "eye scream.ogg";
        public static AudioClip? EyeScream => EyeScreamEnable ? Instance.eyeScream : null;
    }

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

        fallDeathEnable = Config.Bind(
            "Sounds",
            "FallDeath",
            true,
            "Adds scream sound effect on death by Gravity or OutOfBoundsTrigger"
        );
        freddyFazbearEnable = Config.Bind(
            "Sounds",
            "FreddyFazbear",
            true,
            "Adds Five Nights at Freddys game over music when pulling apparatus"
        );
        eyeScreamEnable = Config.Bind(
            "Sounds",
            "EyeScream",
            true,
            "Replaces MouthDog anger sound with Eye Of Cthulhu from Terraria"
        );

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger.LogDebug("Patching...");
        Harmony.PatchAll();
        Logger.LogDebug("Finished patching!");

        Logger.LogDebug("Loading sounds...");
        fallDeath = AudioManager.LoadSound(rel(Sounds.FALL_DEATH));
        freddyFazbear = AudioManager.LoadSound(rel(Sounds.FREDDY_FAZBEAR));
        eyeScream = AudioManager.LoadSound(rel(Sounds.EYE_SCREAM));
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
