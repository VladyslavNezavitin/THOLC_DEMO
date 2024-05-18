using UnityEngine;

public class OpenableClosableSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _openSound;
    [SerializeField] private SoundPlaybackData _closeSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private IOpenable _openable;
    private IClosable _closable;

    private void Awake()
    {
        _openable = GetComponent<IOpenable>();
        _closable = GetComponent<IClosable>();
    }

    private void OnEnable()
    {
        if (_openable != null)
            _openable.Opened += Openable_OnOpened;
        
        if (_closable != null)
            _closable.Closed += Closable_OnClosed;
    }

    private void OnDisable()
    {
        if (_openable != null)
            _openable.Opened -= Openable_OnOpened;

        if (_closable != null)
            _closable.Closed -= Closable_OnClosed;
    }

    private void Openable_OnOpened() =>
        SoundManager.PlaySound(_openSound, transform.position);

    private void Closable_OnClosed() =>
        SoundManager.PlaySound(_closeSound, transform.position);
}
