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
        AudioManager.PlayClip(
            audioClip,
            audioSource => audioSource.volume = 0.75f,
            _ => GameNetworkManager.Instance?.localPlayerController?.isInsideFactory == true
        );
        AudioManager.PlayClip(
            audioClip,
            audioSource =>
            {
                audioSource.volume = 0.2f;
                audioSource.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 2000f;
            },
            _ => GameNetworkManager.Instance?.localPlayerController?.isInsideFactory != true
        );
    }
}
