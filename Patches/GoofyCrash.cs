using HarmonyLib;
using LethalModUtils;
using UnityEngine;

namespace MySoundReplacements.Patches;

[HarmonyPatch(typeof(VehicleController), nameof(VehicleController.DestroyCar))]
public class GoofyCrashPatch
{
    // ReSharper disable once UnusedMember.Local
    private static void Prefix(ref VehicleController __instance)
    {
        var clip = MySoundReplacements.Sounds.GoofyCrash;
        if (!__instance.carDestroyed && clip != null)
            clip.PlayAt(__instance.transform).SetRange(50f);
    }
}
