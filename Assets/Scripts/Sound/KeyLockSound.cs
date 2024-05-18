using UnityEngine;

public class KeyLockSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _interactionSound; 
    [SerializeField] private SoundPlaybackData _keyInsertionSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private KeyLock _lock;

    private void Awake() => _lock = GetComponent<KeyLock>();
    private void OnEnable()
    {
        _lock.Interacted += Lock_OnInteracted;
        _lock.KeyInserted += Lock_OnKeyInserted;
    }

    private void OnDisable()
    {
        _lock.Interacted -= Lock_OnInteracted;
        _lock.KeyInserted -= Lock_OnKeyInserted;
    }

    private void Lock_OnKeyInserted() =>
        SoundManager.PlaySound(_keyInsertionSound, transform.position);

    private void Lock_OnInteracted() =>
        SoundManager.PlaySound(_interactionSound, transform.position);
}
