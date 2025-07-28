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

    private ConfigEntry<bool>? mimicDeathEnable;
    private AudioClip? mimicDeath;

    private ConfigEntry<bool>? goofyCrashEnable;
    private AudioClip? goofyCrash;

    private ConfigEntry<bool>? maskPiggiesEnable;
    private AudioClip? maskPiggies;

    private ConfigEntry<bool>? redSuckerEnable;
    private AudioClip? redSucker;

    private ConfigEntry<bool>? weBeesEnable;
    private AudioClip? weBees;

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

        internal static bool MimicDeathEnable => Instance is { mimicDeathEnable.Value: true };
        internal const string MIMIC_DEATH = "mimicDeath.wav";
        public static AudioClip? MimicDeath => MimicDeathEnable ? Instance.mimicDeath : null;

        internal static bool GoofyCrashEnable => Instance is { goofyCrashEnable.Value: true };
        internal const string GOOFY_CRASH = "goofyCrash.wav";
        public static AudioClip? GoofyCrash => GoofyCrashEnable ? Instance.goofyCrash : null;

        internal static bool MaskPiggiesEnable => Instance is { maskPiggiesEnable.Value: true };
        internal const string MASK_PIGGIES = "maskPiggies.wav";
        public static AudioClip? MaskPiggies => MaskPiggiesEnable ? Instance.maskPiggies : null;

        internal static bool RedSuckerEnable => Instance is { redSuckerEnable.Value: true };
        internal const string RED_SUCKER = "redSucker.wav";
        public static AudioClip? RedSucker => RedSuckerEnable ? Instance.redSucker : null;

        internal static bool WeBeesEnable => Instance is { weBeesEnable.Value: true };
        internal const string WE_BEES = "weBees.wav";
        public static AudioClip? WeBees => WeBeesEnable ? Instance.weBees : null;
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
        mimicDeathEnable = Config.Bind(
            "Sounds",
            "MimicDeath",
            true,
            "Adds the Mimic death sound with The MIMIICCCC (MR BEASTTTT) (no CLICKBAIT) (REAL!!!!!!) meme from Five Nights at Freddy's"
        );
        goofyCrashEnable = Config.Bind(
            "Sounds",
            "GoofyCrash",
            true,
            "Replaces the cruiser crash sound with Goofy dying while listening to Post Malone (RIP)"
        );
        maskPiggiesEnable = Config.Bind(
            "Sounds",
            "MaskPiggies",
            true,
            "Replaces the Mask Hornets sound with the Bad Piggies theme from Angry Birds (the piggies are pretty bad)"
        );
        redSuckerEnable = Config.Bind(
            "Sounds",
            "RedSucker",
            true,
            "Replaces the Sapsucker scream sound with the sound of Red from Angry Birds screaming (ya hooya!)"
        );
        weBeesEnable = Config.Bind(
            "Sounds",
            "WeBees",
            true,
            "Replaces the Circuit Bees angry sound with the we can be bees meme from Invincible (this is good news, Mark)"
        );

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);
        Logger.LogDebug("Patching...");
        Harmony.PatchAll();
        Logger.LogDebug("Finished patching!");

        Logger.LogDebug("Loading sounds...");
        fallDeath = AudioManager.LoadSound(rel(Sounds.FALL_DEATH));
        freddyFazbear = AudioManager.LoadSound(rel(Sounds.FREDDY_FAZBEAR));
        eyeScream = AudioManager.LoadSound(rel(Sounds.EYE_SCREAM));
        mimicDeath = AudioManager.LoadSound(rel(Sounds.MIMIC_DEATH));
        goofyCrash = AudioManager.LoadSound(rel(Sounds.GOOFY_CRASH));
        maskPiggies = AudioManager.LoadSound(rel(Sounds.MASK_PIGGIES));
        redSucker = AudioManager.LoadSound(rel(Sounds.RED_SUCKER));
        weBees = AudioManager.LoadSound(rel(Sounds.WE_BEES));
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
