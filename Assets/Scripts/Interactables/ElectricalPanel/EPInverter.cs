using UnityEngine;

public class EPInverter : EPElement
{
    private EPInverterView _view;

    private void Awake() => _view = GetComponent<EPInverterView>();

    protected override void OnInputChanged()
    {
        bool isInPowered = (CurrentInput & InputSides) > 0;

        CurrentOutput = isInPowered ? 0 : OutputSides;
        _view.UpdateVisual(isInPowered);
    }
}
