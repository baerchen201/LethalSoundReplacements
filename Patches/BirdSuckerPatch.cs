using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(GiantKiwiAI), nameof(GiantKiwiAI.Screech))]
public static class BirdSuckerPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref GiantKiwiAI __instance, ref bool enraged)
    {
        var clip = MySoundReplacements.Sounds.BirdSucker;
        if (clip == null || !enraged)
            return;
        var instance = __instance;
        AudioManager.PlayLoopingClipAtWhile(
            clip,
            _ => instance.attacking,
            __instance.transform,
            audioSource =>
            {
                audioSource.maxDistance = 50f;
                audioSource.rolloffMode = AudioRolloffMode.Linear;
                audioSource.spatialBlend = 1f;
            }
        );
    }
}
