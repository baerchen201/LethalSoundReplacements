using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(RedLocustBees), nameof(RedLocustBees.Start))]
public class WeBeesPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref RedLocustBees __instance)
    {
        __instance.enemyType.audioClips =
        [
            MySoundReplacements.Sounds.WeBees,
            MySoundReplacements.Sounds.WeBees,
        ];
    }
}
