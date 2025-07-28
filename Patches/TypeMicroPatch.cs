using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(MicrowaveItem), nameof(MicrowaveItem.TurnOnMicrowave))]
public class TypeMicroPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Prefix(ref MicrowaveItem __instance)
    {
        __instance.whirringAudio.clip = MySoundReplacements.Sounds.TypeMicro;
    }
}
