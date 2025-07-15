using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.enterChaseMode))]
public class DogPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Prefix(ref MouthDogAI __instance)
    {
        var audioClip = MySoundReplacements.Instance.eyeScream;
        if (audioClip == null)
            return;
        __instance.screamSFX = audioClip;
    }
}
