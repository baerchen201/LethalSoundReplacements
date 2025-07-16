using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.RadiationWarningHUD))]
public class LightOffPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix()
    {
        var audioClip = MySoundReplacements.Sounds.FreddyFazbear;
        if (audioClip == null)
            return;
        AudioManager.PlaySingleClipInside(audioClip, audioSource => audioSource.volume = 0.75f);
        AudioManager.PlaySingleClipInside(
            audioClip,
            audioSource =>
            {
                audioSource.volume = 0.2f;
                audioSource.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 2000f;
            },
            false
        );
    }
}
