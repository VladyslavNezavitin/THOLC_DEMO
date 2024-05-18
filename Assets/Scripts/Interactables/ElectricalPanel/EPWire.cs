using UnityEngine;

public sealed class EPWire : EPElement
{
    [SerializeField] private EPElementView _view;

    protected override void OnInputChanged()
    {
        CurrentOutput = IsEmitter || CurrentInput > 0 ? OutputSides : 0;
        _view.IsPowered = IsEmitter || CurrentInput > 0;
    }
}