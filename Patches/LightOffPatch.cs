using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(LungProp), nameof(LungProp.DisconnectFromMachinery))]
public class LightOffPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Postfix(ref LungProp __instance)
    {
        __instance.roundManager.StartCoroutine(a());
    }

    private static IEnumerator a()
    {
        var audioClip = MySoundReplacements.Instance.freddyFazbear;
        if (audioClip == null)
            yield break;
        yield return new WaitForSeconds(3f);
        AudioManager.PlaySingleClipInside(audioClip, audioSource => audioSource.volume = 0.75f);
        AudioManager.PlaySingleClipInside(
            audioClip,
            audioSource =>
            {
                audioSource.volume = 0.2f;
                audioSource.gameObject.AddComponent<AudioLowPassFilter>().cutoffFrequency = 2000f;
            },
            false
        );
    }
}
