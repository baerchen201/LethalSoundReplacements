using HarmonyLib;
using LethalModUtils;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(HUDManager), nameof(HUDManager.RadiationWarningHUD))]
public class LightOffPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref HUDManager __instance)
    {
        var audioClip = MySoundReplacements.Sounds.FreddyFazbear;
        if (audioClip == null)
            return;
        audioClip
            .PlayAt(
                __instance.transform,
                player =>
                {
                    if (GameNetworkManager.Instance?.localPlayerController?.isInsideFactory == true)
                    {
                        player.Volume = 0.75f;
                        player.BypassEffects = false;
                    }
                    else
                    {
                        player.Volume = 0.2f;
                        player.BypassEffects = true;
                    }
                }
            )
            ._audioSource.gameObject.AddComponent<AudioLowPassFilter>()
            .cutoffFrequency = 2000f;
    }
}
