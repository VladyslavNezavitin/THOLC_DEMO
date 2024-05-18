using UnityEngine;

public class ValveSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _rotationSound;
    private Valve _valve;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private SoundPlaybackData _rotationPlaybackData;

    private void Awake() => _valve = GetComponent<Valve>();
    private void OnEnable()
    {
        _valve.RotationStarted += Valve_OnRotationStarted;
        _valve.RotationStopped += Valve_OnRotationStopped;
    }

    private void OnDisable()
    {
        _valve.RotationStarted -= Valve_OnRotationStarted;
        _valve.RotationStopped -= Valve_OnRotationStopped;
    }

    private void Valve_OnRotationStarted()
    {
        _rotationPlaybackData = _rotationSound.GetClone();
        _rotationPlaybackData.loop = true;

        SoundManager.PlaySound(_rotationPlaybackData, transform.position);
    }

    private void Valve_OnRotationStopped() => _rotationPlaybackData.interruptionRequested = true;
}