using UnityEngine;

public class FocusObject : Interactable, IControlInteractable
{
    [SerializeField] private Transform _focusPoint;
    [SerializeField] private Transform _playerPoint;

    public InteractionControlData GetControlData() => new InteractionControlData()
    {
        objectToLookAt = _focusPoint,
        objectToFollow = _playerPoint,
        freezeMovement = true
    };

    protected override void InteractInternal(InteractionData data)
    {
        data.handler.Handle(this);
    }
}