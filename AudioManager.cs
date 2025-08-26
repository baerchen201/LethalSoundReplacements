using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace MySoundReplacements;

public static class AudioManager
{
    public static void PlayClip(
        AudioClip clip,
        Vector3 origin,
        Action<AudioSource>? audioSourceModifier = null,
        Func<AudioSource, bool>? muteCondition = null,
        Func<AudioSource, bool>? stopCondition = null
    )
    {
        MySoundReplacements.Logger.LogDebug(
            $">> PlayClip({clip}, {origin}, {audioSourceModifier}, {muteCondition}, {stopCondition})"
        );
        CreateAudioSource(origin, null, out var audioSource);
        audioSourceModifier?.Invoke(audioSource);
        PlayClipOn(
            clip,
            audioSource,
            _CleanUpAudioSourceGameObject,
            _CallbackManager(audioSource, muteCondition, stopCondition)
        );
    }

    public static void PlayClip(
        AudioClip clip,
        Transform parentTo,
        Action<AudioSource>? audioSourceModifier = null,
        Func<AudioSource, bool>? muteCondition = null,
        Func<AudioSource, bool>? stopCondition = null
    )
    {
        MySoundReplacements.Logger.LogDebug(
            $">> PlayClip({clip}, {parentTo}, {audioSourceModifier}, {muteCondition}, {stopCondition})"
        );
        CreateAudioSource(parentTo, out var audioSource);
        audioSourceModifier?.Invoke(audioSource);
        PlayClipOn(
            clip,
            audioSource,
            _CleanUpAudioSourceGameObject,
            _CallbackManager(audioSource, muteCondition, stopCondition)
        );
    }

    public static void PlayClip(
        AudioClip clip,
        Action<AudioSource>? audioSourceModifier = null,
        Func<AudioSource, bool>? muteCondition = null,
        Func<AudioSource, bool>? stopCondition = null
    )
    {
        MySoundReplacements.Logger.LogDebug(
            $">> PlayClip({clip}, {audioSourceModifier}, {muteCondition}, {stopCondition})"
        );
        CreateAudioSource(default, null, out var audioSource);
        audioSourceModifier?.Invoke(audioSource);
        PlayClipOn(
            clip,
            audioSource,
            _CleanUpAudioSourceGameObject,
            _CallbackManager(audioSource, muteCondition, stopCondition)
        );
    }

    private static void CreateAudioSource(
        Vector3 origin,
        Transform? parentTo,
        out AudioSource audioSource
    )
    {
        var audioSourceObject = new GameObject();
        audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSourceObject.transform.position = origin;
        audioSourceObject.transform.parent = parentTo;
    }

    private static void CreateAudioSource(Transform origin, out AudioSource audioSource)
    {
        var audioSourceObject = new GameObject();
        audioSource = audioSourceObject.AddComponent<AudioSource>();
        audioSourceObject.transform.position = origin.position;
        audioSourceObject.transform.parent = origin;
    }

    private static void _CleanUpAudioSourceGameObject(AudioSource audioSource)
    {
        if (audioSource && audioSource.gameObject)
            Object.Destroy(audioSource.gameObject);
    }

    public static void PlayClipOn(
        AudioClip clip,
        AudioSource audioSource,
        Action<AudioSource>? onDone = null,
        Action<AudioSource>? update = null
    )
    {
        MySoundReplacements.Logger.LogDebug(
            $">> PlayClipOn({clip}, {audioSource}, {onDone}, {update})"
        );
        audioSource.clip = clip;
        StartOfRound.Instance.StartCoroutine(_PlayingSoundCoroutine(audioSource, onDone, update));
    }

    private static IEnumerator _PlayingSoundCoroutine(
        AudioSource audioSource,
        Action<AudioSource>? onDone,
        Action<AudioSource>? update
    )
    {
        MySoundReplacements.Logger.LogDebug(
            $">> PlayingSoundCoroutine({audioSource}, {onDone}, {update})"
        );
        if (audioSource)
            audioSource.Play();
        while (!audioSource || !audioSource.isPlaying)
        {
            update?.Invoke(audioSource);
            yield return null;
        }
        onDone?.Invoke(audioSource);
    }

    private static Action<AudioSource>? _CallbackManager(
        AudioSource audioSource,
        Func<AudioSource, bool>? muteCondition,
        Func<AudioSource, bool>? stopCondition
    )
    {
        if (muteCondition == null && stopCondition == null)
            return null;
        var volume = audioSource.volume;
        return _audioSource =>
        {
            _audioSource.volume = muteCondition?.Invoke(_audioSource) == true ? 0f : volume;
            if (stopCondition?.Invoke(_audioSource) == true)
                _audioSource.Stop();
        };
    }

    private const string ERR_UNKNOWN_TYPE = "Unknown file type";

    public static AudioClip? LoadSound(string path)
    {
        var audioType = Path.GetExtension(path).ToLower() switch
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
        {
            MySoundReplacements.Logger.LogWarning(
                $"Error loading {Path.GetFullPath(path)}: {ERR_UNKNOWN_TYPE}"
            );
            return null;
        }

        var webRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
        ((DownloadHandlerAudioClip)webRequest.downloadHandler).streamAudio = !MySoundReplacements
            .Instance
            .LoadIntoRAM;
        webRequest.SendWebRequest();
        while (!webRequest.isDone) { }

        if (webRequest.result != UnityWebRequest.Result.Success)
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

    public static IEnumerator LoadSoundAsync(
        string path,
        Action<AudioClip> onSuccess,
        Action<string?>? onFailure
    )
    {
        var audioType = Path.GetExtension(path).ToLower() switch
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
        {
            MySoundReplacements.Logger.LogWarning(
                $"Error loading {Path.GetFullPath(path)}: {ERR_UNKNOWN_TYPE}"
            );
            onFailure?.Invoke(ERR_UNKNOWN_TYPE);
            yield break;
        }

        var webRequest = UnityWebRequestMultimedia.GetAudioClip(path, audioType);
        ((DownloadHandlerAudioClip)webRequest.downloadHandler).streamAudio = !MySoundReplacements
            .Instance
            .LoadIntoRAM;
        yield return webRequest.SendWebRequest();

        if (webRequest.result != UnityWebRequest.Result.Success)
        {
            MySoundReplacements.Logger.LogError(
                $"Error loading {Path.GetFullPath(path)}: {webRequest.error}"
            );
            onFailure?.Invoke(webRequest.error);
            yield break;
        }

        var audioClip = DownloadHandlerAudioClip.GetContent(webRequest);
        if (audioClip && audioClip.loadState == AudioDataLoadState.Loaded)
        {
            MySoundReplacements.Logger.LogInfo($"Loaded {Path.GetFileName(path)}");
            onSuccess(audioClip);
            yield break;
        }

        MySoundReplacements.Logger.LogWarning(
            $"Error loading {Path.GetFullPath(path)}: {audioClip.loadState}"
        );
        onFailure?.Invoke(null);
    }
}
