using System.Threading.Tasks;
using UnityEngine;

public class MainMenuBoombox : Interactable
{
    [SerializeField] private SoundPlaybackData _backgroundMusic;
    [SerializeField] private bool _playOnStart;
    [SerializeField] private ParticleSystem[] _particles;

    private bool _isPlaying;
    private SoundPlaybackData backgroundMusic;

    protected override void InteractInternal(InteractionData data)
    {
        if (_isPlaying)
            Stop();
        else
            Play();
    }

    private async void Start()
    {
        await Task.Delay(5);

        if (_playOnStart)
            Play();
    }

    private void Play()
    {
        backgroundMusic = _backgroundMusic.GetClone();
        backgroundMusic.loop = true;
        ProjectContext.Instance.SoundManager.PlaySound(backgroundMusic, transform.position);
        _isPlaying = true;

        foreach (var p in _particles)
            p.Play();
    }

    private void Stop()
    {
        backgroundMusic.interruptionRequested = true;
        _isPlaying = false;

        foreach (var p in _particles)
            p.Stop();
    }
}
