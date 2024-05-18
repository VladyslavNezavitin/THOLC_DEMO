using UnityEngine;

[RequireComponent(typeof(Handle))]
public class EPSwitch : EPElement
{
    [SerializeField] private EPElementView _view;
    private Handle _handle;

    private void Awake() => _handle = GetComponent<Handle>();
    private void OnEnable() => _handle.CurrentStepChanged += Handle_OnCurrentStepChanged;
    private void OnDisable() => _handle.CurrentStepChanged -= Handle_OnCurrentStepChanged;

    protected override void RecalculateIOOffset() => CurrentIOOffset = _handle.CurrentStep;

    protected override void OnInputChanged()
    {
        CurrentOutput = CurrentInput > 0 ? OutputSides : 0;
        _view.IsPowered = CurrentInput > 0;
    }

    private void Handle_OnCurrentStepChanged(Handle handle)
    {
        RecalculateIOOffset();
        RecalculateConnections();
    }
}