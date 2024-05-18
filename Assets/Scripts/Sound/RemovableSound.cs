using UnityEngine;

[RequireComponent(typeof(Removable))]
public class RemovableSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _removeSound;
    [SerializeField] private SoundPlaybackData _hitSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private Removable _removable;

    private void Awake() => _removable = GetComponent<Removable>();

    private void OnEnable()
    {
        _removable.Removed += Removable_OnRemoved;
        _removable.ObjectHit += Removable_OnObjectHit;
    }

    private void OnDisable()
    {
        _removable.Removed -= Removable_OnRemoved;
        _removable.ObjectHit -= Removable_OnObjectHit;
    }

    private void Removable_OnRemoved(Removable removable) =>
        SoundManager.PlaySound(_removeSound, transform.position);

    private void Removable_OnObjectHit(float fallDistance)
    {
        float volumeFactor = Mathf.InverseLerp(0f, 2f, fallDistance);

        var hitSound = _hitSound.GetClone();
        hitSound.volume = _hitSound.volume * volumeFactor;
        
        SoundManager.PlaySound(hitSound, transform.position);
    }
}