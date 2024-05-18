using UnityEngine;

public class TrolleySound : MonoBehaviour
{
    [SerializeField] private float _hitSoundCooldown;
    [SerializeField] private AudioClip _interactionSound;
    [SerializeField] private AudioClip _exitSound;
    [SerializeField] private AudioClip _hitSound;

    private AudioSource _source;
    private Trolley _trolley;
    private float _hitSoundCooldownTimer;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _trolley = GetComponent<Trolley>();
    }

    private void OnEnable()
    {
        _trolley.Interacted += Trolley_OnInteracted;
        _trolley.Exited += Trolley_OnExited;
        _trolley.ObjectHit += Trolley_OnObjectHit;
    }

    private void OnDisable()
    {
        _trolley.Interacted -= Trolley_OnInteracted;
        _trolley.Exited -= Trolley_OnExited;
        _trolley.ObjectHit -= Trolley_OnObjectHit;
    }

    private void Update()
    {
        if (!_trolley.IsActive)
            return;

        if (_trolley.CurrentSpeed > 0.5f && !_source.isPlaying)
        {
            _source.Play();
        }
        else if (_trolley.CurrentSpeed < 0.5f && _source.isPlaying)
        {
            _source.Stop();
        }

        if (_hitSoundCooldownTimer > 0)
            _hitSoundCooldownTimer -= Time.deltaTime;
    }

    private void Trolley_OnObjectHit()
    {
        if (_hitSoundCooldownTimer <= 0)
        {
            _source.PlayOneShot(_hitSound, 0.5f);
            _hitSoundCooldownTimer = _hitSoundCooldown;
        }
    }

    private void Trolley_OnExited()
    {
        _source.Stop();
        _source.PlayOneShot(_exitSound, 1f);
    }

    private void Trolley_OnInteracted()
    {
        _source.PlayOneShot(_interactionSound, 1f);
    }
}
