using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioMixerGroup _sfxGroup;
    [SerializeField] private AudioMixerGroup _musicGroup;
    
    private ObjectPool<AudioSource> _sourcePool;
    private List<AudioSource> _activeSources;
    private SceneLoader _sceneLoader;


    public void Initialize(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
        _sourcePool = new ObjectPool<AudioSource>(8, true, transform);
        _activeSources = new List<AudioSource>(8);
    }

    private void Start()
    {
        // for some reason not working in initialize/enable
        ProjectContext.Instance.SoundManager.SetMasterVolume(PlayerPrefs.GetFloat(Constants.MIXER_MASTER_VOLUME, 1f));
        ProjectContext.Instance.SoundManager.SetSFXVolume(PlayerPrefs.GetFloat(Constants.MIXER_SFX_VOLUME, 0.5f));
        ProjectContext.Instance.SoundManager.SetMusicVolume(PlayerPrefs.GetFloat(Constants.MIXER_MUSIC_VOLUME, 0.5f));
    }

    public void Enable()
    {
        _sceneLoader.SceneLoadingStarted += SceneLoader_OnSceneLoadingStarted;
    }

    public void Disable()
    {
        _sceneLoader.SceneLoadingStarted -= SceneLoader_OnSceneLoadingStarted;
    }

    private void SceneLoader_OnSceneLoadingStarted()
    {
        AudioSource[] activeSources = _activeSources.ToArray();
        foreach (var source in activeSources)
        {
            source.Stop();
            _sourcePool.Recycle(source);
            _activeSources.Remove(source);
        }
    }

    private IEnumerator MasterFadeRoutine(float from, float to)
    {
        bool isDone = false;
        Debug.Log(from + " " + to);
        LeanTween.reset();
        LeanTween.value(from, to, 2f)
        .setOnUpdate((p) => { SetMasterVolume(p, false); })
        .setOnComplete(() => isDone = true);

        while (!isDone)
            yield return null;
    }


    #region VolumeAdjustment

    public void SetSFXVolume(float volume, bool saveInPlayerPrefs = true)
    {
        volume = Mathf.Clamp(volume, 0.000001f, 1f);
        _mixer.SetFloat(Constants.MIXER_SFX_VOLUME, Mathf.Log10(volume) * 20);

        if (saveInPlayerPrefs)
            PlayerPrefs.SetFloat(Constants.MIXER_SFX_VOLUME, volume);
    }

    public void SetMusicVolume(float volume, bool saveInPlayerPrefs = true)
    {
        volume = Mathf.Clamp(volume, 0.000001f, 1f);
        _mixer.SetFloat(Constants.MIXER_MUSIC_VOLUME, Mathf.Log10(volume) * 20);

        if (saveInPlayerPrefs)
            PlayerPrefs.SetFloat(Constants.MIXER_MUSIC_VOLUME, volume);
    }

    public void SetMasterVolume(float volume, bool saveInPlayerPrefs = true)
    {
        volume = Mathf.Clamp(volume, 0.000001f, 1f);
        _mixer.SetFloat(Constants.MIXER_MASTER_VOLUME, Mathf.Log10(volume) * 20);
        
        if (saveInPlayerPrefs)
            PlayerPrefs.SetFloat(Constants.MIXER_MASTER_VOLUME, volume);
    }

    #endregion

    #region SoundPlayback

    public void PlayRandomSound(SoundPlaybackData[] sounds, Vector3 position) =>
        PlaySound(sounds[UnityEngine.Random.Range(0, sounds.Length)], position);

    public void PlaySound(SoundPlaybackData sound, Vector3 position) => 
        StartCoroutine(PlaybackRoutine(sound, position));

    private IEnumerator PlaybackRoutine(SoundPlaybackData data, Vector3 position)
    {
        AudioSource source = _sourcePool.Get();
        _activeSources.Add(source);

        switch (data.type)
        {
            case SoundType.SoundFX:
                source.outputAudioMixerGroup = _sfxGroup;
                source.spatialBlend = 1f;
                break;
            case SoundType.Music:
                source.outputAudioMixerGroup = _musicGroup;
                source.spatialBlend = 0f;
                break;
        }

        source.transform.position = position;
        source.loop = data.loop;
        source.clip = data.clip;
        source.volume = data.volume;
        source.pitch = UnityEngine.Random.Range(1f - data.pitchRange, 1f + data.pitchRange);

        source.Play();

        while (source.isPlaying && !data.interruptionRequested)
            yield return null;

        //LeanTween.value(gameObject, (v) => source.volume = v, source.volume, 0f, 1f);

        source.Stop();

        _activeSources.Remove(source);
        _sourcePool.Recycle(source);
    }

    #endregion
}

[Serializable]
public class SoundPlaybackData
{
    public AudioClip clip;
    public SoundType type = SoundType.SoundFX;
    public float volume = 1f;
    public float pitchRange = 0f;
    public bool loop;
    public bool interruptionRequested;

    public SoundPlaybackData GetClone() => (SoundPlaybackData)MemberwiseClone();
}

public enum SoundType
{
    SoundFX,
    Music
}