using System;
using System.Collections;
using UnityEngine;

public sealed class Drawer : Interactable, IOpenable, IClosable
{
    public event Action Opened;
    public event Action Closed;

    [SerializeField] private float _openTime = 1f;
    [SerializeField] private float _openDistance = 0.5f;
    [SerializeField] private Axis _openAxis;

    private IInteractableController _controller;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (TryGetComponent(out _controller))
            _controller.Initialize(this, this);
    }

    protected override bool ValidateInteractionInternal(Item item) => CanInteract();
    protected override void InteractInternal(InteractionData data)
    {
        if (CanInteract())
            OpenClose();
    }

    void IOpenable.Open()
    {
        if (!IsOpen)
        {
            OpenClose();
        }
    }

    void IClosable.Close()
    {
        if (IsOpen)
        {
            OpenClose();
        }
    }

    private void OpenClose()
    {
        if (LeanTween.isTweening(gameObject))
            return;

        Vector3 targetPosition = transform.position + 
            _openAxis.ToTransformDirection(transform) * _openDistance * (IsOpen ? -1f : 1f);

        LeanTween.move(gameObject, targetPosition, _openTime)
        .setEaseInOutQuad();

        IsOpen = !IsOpen;

        if (IsOpen)
            Opened?.Invoke();
        else
            Closed?.Invoke();
    }

    private bool CanInteract()
    {
        if (_controller == null)
            return true;

        return _controller.CanInteract;
    }
}