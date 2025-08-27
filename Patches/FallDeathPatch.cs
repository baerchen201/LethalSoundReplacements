using GameNetcodeStuff;
using HarmonyLib;
using LethalModUtils;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayerClientRpc))]
internal class FallDeathPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref PlayerControllerB __instance, ref int causeOfDeath)
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
        var audioClip = MySoundReplacements.Sounds.FallDeath;
        if (audioClip == null)
            return;
        var audioPlayer = parentTo
            ? audioClip.PlayAt(parentTo!, b)
            : audioClip.PlayAt(origin, StartOfRound.Instance.transform, b);
        audioPlayer.SetRange(50f);
        audioPlayer.Volume = 0.5f;

        return;

        void b(Audio.AudioPlayer player)
        {
            if (player.State != Audio.AudioPlayer.PlayerState.Playing)
                player.Cancel();
        }
    }
}

[HarmonyPatch(typeof(OutOfBoundsTrigger), nameof(OutOfBoundsTrigger.OnTriggerEnter))]
internal class OutOfBoundsPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref OutOfBoundsTrigger __instance, ref Collider other)
    {
        if (
            (__instance.disableWhenRoundStarts && !StartOfRound.Instance.inShipPhase)
            || !other.TryGetComponent<PlayerControllerB>(out var player)
        )
            return;
        FallDeathPatch.a(player.transform.position);
    }
}
