using UnityEngine;

public class EPGate : EPElement
{
    [SerializeField] private EPDirection _gateSide;
    private EPGateView _view;

    private void Awake() => _view = GetComponent<EPGateView>();

    protected override void OnInputChanged()
    {
        bool isInPowered = (CurrentInput & InputSides & ~_gateSide) > 0;
        bool isGatePowered = (CurrentInput & _gateSide) > 0;
        CurrentOutput = isInPowered && isGatePowered ? OutputSides : 0;
        _view.UpdateVisual(isInPowered, isGatePowered);
    }
}