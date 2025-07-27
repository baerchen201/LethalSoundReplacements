using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(VehicleController), nameof(VehicleController.DestroyCar))]
public class GoofyCrashPatch
{
    private static void Prefix(ref VehicleController __instance)
    {
        var clip = MySoundReplacements.Sounds.GoofyCrash;
        if (!__instance.carDestroyed && clip != null)
        {
            AudioManager.PlaySingleClipAt(
                clip,
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
}
