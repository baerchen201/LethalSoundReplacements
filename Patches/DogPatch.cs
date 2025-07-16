using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.enterChaseMode), MethodType.Enumerator)]
public class DogPatch
{
    // ReSharper disable once UnusedMember.Local
    private static IEnumerable<CodeInstruction> Transpiler(
        IEnumerable<CodeInstruction> instructions
    ) =>
        new CodeMatcher(instructions)
            .MatchForward(
                false,
                new CodeMatch(
                    OpCodes.Ldfld,
                    AccessTools.Field(typeof(MouthDogAI), nameof(MouthDogAI.screamSFX))
                )
            )
            .Advance(1)
            .Insert(
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(DogPatch), nameof(a)))
            )
            .InstructionEnumeration();

    private static AudioClip a(AudioClip _audioClip) =>
        MySoundReplacements.Sounds.EyeScream ?? _audioClip;
}
