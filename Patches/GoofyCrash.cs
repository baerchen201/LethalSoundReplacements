using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(VehicleController), nameof(VehicleController.DestroyCar))]
public class GoofyCrashPatch
{
    private static void Prefix(ref VehicleController __instance)
    {
        if (!__instance.carDestroyed)
        {
            AudioManager.PlaySingleClipAt(
                MySoundReplacements.Sounds.GoofyCrash,
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
