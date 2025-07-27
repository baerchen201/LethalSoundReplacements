using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(ButlerBeesEnemyAI), nameof(ButlerBeesEnemyAI.Start))]
public static class MaskPiggiesPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref ButlerBeesEnemyAI __instance)
    {
        var clip = MySoundReplacements.Sounds.MaskPiggies;
        if (!__instance.buzzing || clip == null)
        {
            MySoundReplacements.Logger.LogWarning(
                $"Couldn't replace AudioClip on Mask Hornets: {__instance} AudioSource:{__instance.buzzing} AudioClip:{clip}"
            );
            return;
        }

        __instance.buzzing.clip = clip;
        __instance.buzzing.Play();
    }
}

[HarmonyPatch(typeof(ButlerBeesEnemyAI), nameof(ButlerBeesEnemyAI.DoAIInterval))]
public static class MaskPiggiesPitchPatch
{
    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions
    ) =>
        instructions.Select(i =>
            ReferenceEquals(
                i.operand,
                AccessTools.PropertySetter(typeof(AudioSource), nameof(AudioSource.pitch))
            )
                ? new CodeInstruction(OpCodes.Pop)
                : i
        );
}
