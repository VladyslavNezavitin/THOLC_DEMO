using System;
using System.Collections;
using UnityEngine;


public class Switch : Interactable
{
    public event Action TurnedOn, TurnedOff;

    [SerializeField] private Axis _axis;
    [SerializeField] private Transform _handle;
    [SerializeField] private ItemPlace _fusePlace;
    [SerializeField] private bool _invertState;

    private bool _isSwitching;

    private bool _state;
    public bool State
    {
        get => _state;
        private set
        {
            if (_isSwitching)
                return;

            _state = value;

            float targetAngle = _state ? 180 : -180;
            if (_invertState) targetAngle *= -1f;

            StartCoroutine(SwitchRoutine(targetAngle));
        }
    }

    private void Start()
    {
        _state = _invertState;

        if (State)
            TurnedOn?.Invoke();
        else
            TurnedOff?.Invoke();
    }

    protected override bool ValidateInteractionInternal(Item item) => _fusePlace.RequiredItemPlaced;
    protected override void InteractInternal(InteractionData data)
    {
        if (ValidateInteraction(data.item))
            State = !State;
    }

    private IEnumerator SwitchRoutine(float targetAngle)
    {
        _isSwitching = true;

        yield return StartCoroutine(Routines.TransformRotationRoutine(_handle, _axis, targetAngle, 1.5f));

        if (State)
            TurnedOn?.Invoke();
        else
            TurnedOff?.Invoke();

        _isSwitching = false;
    }
}
