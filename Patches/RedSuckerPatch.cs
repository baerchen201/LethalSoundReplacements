using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(GiantKiwiAI), nameof(GiantKiwiAI.Start))]
public class RedSuckerPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref GiantKiwiAI __instance)
    {
        __instance.screamSFX =
        [
            MySoundReplacements.Sounds.RedSucker,
            MySoundReplacements.Sounds.RedSucker,
        ];
    }
}
