using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(PlayerController))]
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private float _crouchFootstepCooldown;
    [SerializeField] private float _crouchFootstepVolumeScale;
    [SerializeField] private float _walkFootstepCooldown;
    [SerializeField] private float _walkFootstepVolumeScale;
    [SerializeField] private Sound[] _sounds;

    private AudioSource _source;
    private PlayerController _controller;
    private Dictionary<SoundType, AudioClip[]> _soundMap;

    private float _footstepTimer;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _controller = GetComponent<PlayerController>();

        _soundMap = new Dictionary<SoundType, AudioClip[]>();
        foreach (var s in _sounds)
            _soundMap.Add(s.type, s.clips);
    }

    private void OnEnable()
    {
        _controller.PlayerLanded += Controller_OnPlayerLanded;
    }

    private void OnDisable()
    {
        _controller.PlayerLanded -= Controller_OnPlayerLanded;
    }

    private void Update()
    {
        if (_footstepTimer <= 0f)
        {
            _footstepTimer = _controller.IsCrouching ?
                _crouchFootstepCooldown : _walkFootstepCooldown;

            HandleFootsteps();
        }

        _footstepTimer -= Time.deltaTime;
    }

    private void HandleFootsteps()
    {
        if (_controller.IsOnGround &&
            (Mathf.Abs(_controller.Velocity.x) > 1f || Mathf.Abs(_controller.Velocity.z) > 1f))
        {
            AudioClip[] clips = _controller.SurfaceType switch
            {
                SurfaceType.Stone => _soundMap[SoundType.Footstep_Stone],
                SurfaceType.Sand => _soundMap[SoundType.Footstep_Sand],
                SurfaceType.Metal => _soundMap[SoundType.Footstep_Metal],
                SurfaceType.Wood => _soundMap[SoundType.Footstep_Wood],
                SurfaceType.Vent => _soundMap[SoundType.Footstep_Vent],
                _ => _soundMap[SoundType.Footstep_Stone]
            };

            PlayRandom(clips);
        }
    }

    private void Controller_OnPlayerLanded()
    {
        AudioClip[] clips = _controller.SurfaceType switch
        {
            SurfaceType.Stone => _soundMap[SoundType.Land_Stone],
            SurfaceType.Sand => _soundMap[SoundType.Land_Sand],
            SurfaceType.Metal => _soundMap[SoundType.Land_Metal],
            SurfaceType.Wood => _soundMap[SoundType.Land_Wood],
            SurfaceType.Vent => _soundMap[SoundType.Land_Vent],
            _ => _soundMap[SoundType.Land_Stone]
        };

        PlayRandom(clips);

        _footstepTimer = _controller.IsCrouching ? 
            _crouchFootstepCooldown : _walkFootstepCooldown;
    }

    private void PlayRandom(AudioClip[] clips)
    {
        int index = UnityEngine.Random.Range(0, clips.Length);
        _source.pitch = UnityEngine.Random.Range(0.85f, 1.15f);

        float volumeScale = _controller.IsCrouching ? _crouchFootstepVolumeScale : _walkFootstepVolumeScale;
        _source.PlayOneShot(clips[index], volumeScale);
    }

    [Serializable]
    private struct Sound
    {
        public string name;
        public SoundType type;
        public AudioClip[] clips;
    }

    private enum SoundType
    {
        Footstep_Stone,
        Footstep_Sand,
        Footstep_Metal,
        Footstep_Wood,
        Footstep_Vent,
        Land_Stone,
        Land_Sand,
        Land_Metal,
        Land_Wood,
        Land_Vent,
    }
}
