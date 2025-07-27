using HarmonyLib;
using UnityEngine;

namespace MySoundReplacements.Patches
{
    [HarmonyPatch(typeof(VehicleController))]
    [HarmonyPatch("CarReactToObstacle")]
    [HarmonyPatch(
        new[]
        {
            typeof(Vector3),
            typeof(Vector3),
            typeof(Vector3),
            typeof(CarObstacleType),
            typeof(float),
            typeof(EnemyAI),
            typeof(bool),
        }
    )]
    public static class GoofyCrashPatch
    {
        private static void Prefix(CarObstacleType type, Vector3 position)
        {
            // Only play the sound for specific crash types if you want
            if (type == CarObstacleType.Player || type == CarObstacleType.Object)
            {
                var audio = MySoundReplacements.Instance.GetComponent<AudioSource>();
                if (MySoundReplacements.Sounds.GoofyCrash != null && audio != null)
                {
                    audio.PlayOneShot(MySoundReplacements.Sounds.GoofyCrash);
                    MySoundReplacements.Logger.LogDebug(
                        "Played GoofyCrash.wav in CarReactToObstacle"
                    );
                }
            }
        }
    }
}
