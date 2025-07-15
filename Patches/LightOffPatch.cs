using HarmonyLib;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(LungProp), nameof(LungProp.DisconnectFromMachinery))]
public class LightOffPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix()
    {
        var audioClip = MySoundReplacements.Instance.freddyFazbear;
        if (audioClip == null)
            return;
        AudioManager.PlaySingleClipGlobal(audioClip);
    }
}
