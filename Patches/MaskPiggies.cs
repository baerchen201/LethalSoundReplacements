using System.Collections;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches
{
    [HarmonyPatch(typeof(ButlerBeesEnemyAI), nameof(ButlerBeesEnemyAI.DoAIInterval))]
    public static class MaskPiggiesPatch
    {
        private static readonly ConditionalWeakTable<ButlerBeesEnemyAI, PlayedState> _playedTable =
            new();

        private class PlayedState
        {
            public bool hasSwappedAndPlayed = false;
        }

        private static void Prefix(ButlerBeesEnemyAI __instance)
        {
            if (__instance.buzzing == null || MySoundReplacements.Sounds.MaskPiggies == null)
                return;

            var state = _playedTable.GetOrCreateValue(__instance);

            if (!state.hasSwappedAndPlayed)
            {
                var source = __instance.buzzing;
                source.clip = MySoundReplacements.Sounds.MaskPiggies;
                source.loop = true;
                source.Play();

                state.hasSwappedAndPlayed = true;
                MySoundReplacements.Logger.LogDebug(
                    "🐝 MaskPiggies: sound swapped and played once."
                );
            }

            // ✅ Start coroutine from the enemy instance
            __instance.StartCoroutine(ResetPitchNextFrame(__instance));
        }

        private static IEnumerator ResetPitchNextFrame(ButlerBeesEnemyAI instance)
        {
            yield return null; // wait until end of frame
            if (
                instance != null
                && instance.buzzing != null
                && instance.buzzing.clip == MySoundReplacements.Sounds.MaskPiggies
            )
            {
                instance.buzzing.pitch = 1f;
                MySoundReplacements.Logger.LogDebug("🐝 Pitch forced back to 1f (delayed).");
            }
        }
    }
}
