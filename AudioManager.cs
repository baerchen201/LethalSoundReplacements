using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace MySoundReplacements;

public static class AudioManager
{
    public static void PlaySingleClipAt(
        AudioClip clip,
        Vector3 origin,
        Transform? parentTo = null,
        Action<AudioSource>? audioSourceModifier = null
    )
    {
        CreateAudioSource(origin, parentTo, out var audioSource, out var audioSourceObject);
        audioSourceModifier?.Invoke(audioSource);
        PlaySingleClipOn(clip, audioSource, audioSourceObject);
    }

    public static void PlaySingleClipAt(
        AudioClip clip,
        Transform origin,
        Action<AudioSource>? audioSourceModifier = null
    )
    {
        CreateAudioSource(origin, out var audioSource, out var audioSourceObject);
        audioSourceModifier?.Invoke(audioSource);
        PlaySingleClipOn(clip, audioSource, audioSourceObject);
    }

    public static void PlaySingleClipGlobal(
        AudioClip clip,
        Action<AudioSource>? audioSourceModifier = null
    )
    {
        CreateAudioSource(Vector3.zero, null, out var audioSource, out var audioSourceObject);
        audioSourceModifier?.Invoke(audioSource);
        PlaySingleClipOn(clip, audioSource, audioSourceObject);
    }

    public static void PlaySingleClipInside(
        AudioClip clip,
        Action<AudioSource>? audioSourceModifier = null,
        bool inside = true
    )
    {
        CreateAudioSource(Vector3.zero, null, out var audioSource, out var audioSourceObject);
        audioSourceModifier?.Invoke(audioSource);
        PlaySingleClipOnWhen(clip, audioSource, audioSourceObject, _ => IsLocalPlayer(inside));
    }

    private static bool IsLocalPlayer(bool inside) =>
        GameNetworkManager.Instance != null
        && GameNetworkManager.Instance.localPlayerController != null
        && GameNetworkManager.Instance.localPlayerController.isInsideFactory == inside;

    internal static void CreateAudioSource(
        Vector3 origin,
        Transform? parentTo,
        out AudioSource audioSource,
        out GameObject audioSourceObject
    )
    {
        audioSourceObject = new GameObject();
        audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSourceObject.transform.position = origin;
        audioSourceObject.transform.parent = parentTo;
    }

    internal static void CreateAudioSource(
        Transform origin,
        out AudioSource audioSource,
        out GameObject audioSourceObject
    )
    {
        audioSourceObject = new GameObject();
        audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSourceObject.transform.position = origin.position;
        audioSourceObject.transform.parent = origin;
    }

    public static void PlaySingleClipOn(
        AudioClip clip,
        AudioSource audioSource,
        GameObject audioSourceObject
    )
    {
        audioSource.PlayOneShot(clip);
        MySoundReplacements.Instance.StartCoroutine(
            _CleanUpAfterPlayingAudio(audioSource, audioSourceObject)
        );
    }

    public static void PlaySingleClipOnWhen(
        AudioClip clip,
        AudioSource audioSource,
        GameObject audioSourceObject,
        Func<AudioSource, bool> predicate
    )
    {
        audioSource.PlayOneShot(clip);
        MySoundReplacements.Instance.StartCoroutine(
            _CleanUpAfterPlayingAudioWhen(audioSource, audioSourceObject, predicate)
        );
    }

    private static IEnumerator _CleanUpAfterPlayingAudio(
        AudioSource audioSource,
        GameObject cleanUp
    )
    {
        yield return new WaitUntil(() => !audioSource || !audioSource.isPlaying);
        if (cleanUp)
            Object.Destroy(cleanUp);
    }

    private static IEnumerator _CleanUpAfterPlayingAudioWhen(
        AudioSource audioSource,
        GameObject cleanUp,
        Func<AudioSource, bool> predicate
    )
    {
        var volume = audioSource.volume;
        while (audioSource && audioSource.isPlaying)
        {
            audioSource.volume = predicate(audioSource) ? volume : 0f;
            yield return null;
        }
        if (cleanUp)
            Object.Destroy(cleanUp);
    }

    public static AudioClip? LoadSound(string path)
    {
        AudioType audioType = Path.GetExtension(path).ToLower() switch
        {
            ".ogg" => AudioType.OGGVORBIS,
            ".mp3" => AudioType.MPEG,
            ".wav" => AudioType.WAV,
            ".m4a" => AudioType.ACC,
            ".aiff" => AudioType.AIFF,
            _ => AudioType.UNKNOWN,
        };
        MySoundReplacements.Logger.LogDebug(
            $">> LoadSound({Path.GetFullPath(path)}) audioType:{audioType}"
        );
        if (audioType == AudioType.UNKNOWN)
            return null;

        var webRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
        ((DownloadHandlerAudioClip)webRequest.downloadHandler).streamAudio = !MySoundReplacements
            .Instance
            .LoadIntoRAM;
        webRequest.SendWebRequest();
        while (!webRequest.isDone) { }

        if (webRequest.error != null)
        {
            MySoundReplacements.Logger.LogError(
                $"Error loading {Path.GetFullPath(path)}: {webRequest.error}"
            );
            return null;
        }

        var audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
        if (audioClip && audioClip.loadState == AudioDataLoadState.Loaded)
        {
            MySoundReplacements.Logger.LogInfo($"Loaded {Path.GetFileName(path)}");
            return audioClip;
        }

        MySoundReplacements.Logger.LogWarning(
            $"Error loading {Path.GetFullPath(path)}: {audioClip.loadState}"
        );
        return null;
    }
}
