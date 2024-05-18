using System;
using UnityEngine;

public interface IOpenable
{
    public event Action Opened;
    public bool IsOpen { get; }
    void Open();
}

public interface IClosable
{
    public event Action Closed;
    public bool IsOpen { get; }
    void Close();
}

public sealed class Door : Interactable, IOpenable, IClosable
{
    public event Action Opened;
    public event Action Closed;

    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _openAngle = 90f;
    [SerializeField] private Axis _axis = Axis.Y;

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

    private bool CanInteract()
    {
        if (_controller == null)
            return true;

        return _controller.CanInteract;
    }

    void IOpenable.Open()
    {
        if (!IsOpen)
            OpenClose();
    }

    void IClosable.Close()
    {
        if (IsOpen)
            OpenClose();
    }

    private void OpenClose()
    {
        if (LeanTween.isTweening(gameObject))
            return;

        Vector3 targetRotation = transform.eulerAngles +
            _axis.ToVector() * _openAngle * (IsOpen ? -1f : 1f);

        LeanTween.rotate(gameObject, targetRotation, _animationDuration)
        .setEaseInOutQuad();

        IsOpen = !IsOpen;

        if (IsOpen)
            Opened?.Invoke();
        else
            Closed?.Invoke();

    }
}