using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.killAnimation))]
public class MimicDeathPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Prefix(ref MaskedPlayerEnemy __instance)
    {
        __instance.enemyType.audioClips[0] = MySoundReplacements.Sounds.MimicDeath;
    }
}
