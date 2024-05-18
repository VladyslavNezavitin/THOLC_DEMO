using System;
using UnityEngine;
using CBContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class InputManager : MonoBehaviour
{
    public event Action JumpTriggered, DropItemTriggered;
    public event Action CrouchTriggered, CrouchCanceled;
    public event Action InteractionTriggered, InteractionCanceled;
    public event Action EscapeTriggered;

    private PlayerInputActions _inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MoveInputRAW { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public Vector2 MouseDelta { get; private set; }

    private Vector2 _smoothInputVelocity;
    private float _smoothInputSpeed = .1f;


    private void Awake() => _inputActions = new PlayerInputActions();

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Gameplay.Jump.started += OnJumpActionTriggered;
        _inputActions.Gameplay.DropItem.started += OnDropItemActionTriggered;
        _inputActions.Gameplay.Interact.started += OnInteractionActionTriggered;
        _inputActions.Gameplay.Interact.canceled += OnInteractionActionCanceled;
        _inputActions.Gameplay.Crouch.started += OnCrouchActionTriggered;
        _inputActions.Gameplay.Crouch.canceled += OnCrouchActionCanceled;
        _inputActions.Gameplay.Escape.started += OnEscapeTriggered;
    }

    private void OnDisable()
    {
        _inputActions.Disable();

        _inputActions.Gameplay.Jump.started -= OnJumpActionTriggered;
        _inputActions.Gameplay.DropItem.started -= OnDropItemActionTriggered;
        _inputActions.Gameplay.Interact.started -= OnInteractionActionTriggered;
        _inputActions.Gameplay.Interact.canceled -= OnInteractionActionCanceled;
        _inputActions.Gameplay.Crouch.started -= OnCrouchActionTriggered;
        _inputActions.Gameplay.Crouch.canceled -= OnCrouchActionCanceled;
    }

    private void Update()
    {
        MoveInputRAW = _inputActions.Gameplay.Movement.ReadValue<Vector2>();
        MoveInput = Vector2.SmoothDamp(MoveInput, MoveInputRAW, ref _smoothInputVelocity, _smoothInputSpeed);
        MouseDelta = _inputActions.Gameplay.MouseDelta.ReadValue<Vector2>();
    }

    private void OnJumpActionTriggered(CBContext context) => JumpTriggered?.Invoke();
    private void OnDropItemActionTriggered(CBContext context) => DropItemTriggered?.Invoke();
    private void OnInteractionActionTriggered(CBContext context) => InteractionTriggered?.Invoke();
    private void OnInteractionActionCanceled(CBContext context) => InteractionCanceled?.Invoke();
    private void OnCrouchActionTriggered(CBContext context) => CrouchTriggered?.Invoke();
    private void OnCrouchActionCanceled(CBContext context) => CrouchCanceled?.Invoke();
    private void OnEscapeTriggered(CBContext context) => EscapeTriggered?.Invoke();
}