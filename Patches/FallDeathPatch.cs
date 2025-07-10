using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerClientRpc))]
internal class FallDeathPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(
        ref PlayerControllerB __instance,
        ref int playerId,
        ref bool spawnBody,
        ref Vector3 bodyVelocity,
        ref int causeOfDeath,
        ref int deathAnimation,
        ref Vector3 positionOffset
    )
    {
        if (causeOfDeath != (int)CauseOfDeath.Gravity)
            return;
        if (__instance.deadBody)
            a(__instance.placeOfDeath, __instance.deadBody.gameObject.transform);
        else
            a(__instance.placeOfDeath);
    }

    internal static void a(Vector3 origin, Transform? parentTo = null)
    {
        var audioClip = MySoundReplacements.Instance.fallDeath;
        if (!audioClip)
            return;
        if (parentTo)
            AudioManager.PlaySingleClipAt(audioClip!, parentTo!, b);
        else
            AudioManager.PlaySingleClipAt(audioClip!, origin, null, b);
    }

    internal static void b(AudioSource audioSource)
    {
        audioSource.maxDistance = 50f;
        audioSource.volume = 0.5f;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.spatialBlend = 1f;
    }
}
