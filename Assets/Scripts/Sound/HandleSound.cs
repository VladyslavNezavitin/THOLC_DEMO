using UnityEngine;

[RequireComponent(typeof(Handle))]
public class HandleSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _switchSound;
    private Handle _handle;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;


    private void Awake() => _handle = GetComponent<Handle>();
    private void OnEnable() => _handle.CurrentStepChanged += Handle_OnCurrentStepChanged;
    private void OnDestroy() => _handle.CurrentStepChanged -= Handle_OnCurrentStepChanged;

    private void Handle_OnCurrentStepChanged(Handle handle) =>
        SoundManager.PlaySound(_switchSound, transform.position);
}
