using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerInteractions))]
public class PlayerController : MonoBehaviour
{
    public event Action CrouchCanceled;
    public event Action PlayerLanded;

    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _jumpForce;
    [Range(1, 100), SerializeField] private int _surfaceUpdateRate;
    [SerializeField] private LayerMask _surfaceLayers;

    private InputManager _input;
    private CharacterController _controller;
    private PlayerInteractions _interactions;

    private const float GRAVITY = -9.8f * 3f;
    private float _controllerPrimaryHeight;
    private Vector3 _controllerPrimaryCenter;

    private Transform _followObject;
    private Vector3 _velocity;
    private bool _crouchCancellationRequested;
    private float _surfaceUpdateTimer;
    private float _lastGroundHeight;

    public Vector3 Velocity => _velocity;
    public SurfaceType SurfaceType { get; private set; }
    public float CurrentSpeed { get; private set; }
    public bool IsCrouching { get; private set; }
    public bool IsOnGround { get; private set; }
    public bool IsFrozen { get; private set; }


    private void Awake()
    {
        _input = ProjectContext.Instance.InputManager;
        _controller = GetComponent<CharacterController>();
        _interactions = GetComponent<PlayerInteractions>();

        _controllerPrimaryHeight = _controller.height;
        _controllerPrimaryCenter = _controller.center;

        CurrentSpeed = _walkSpeed;
    }

    private void OnEnable()
    {
        _input.JumpTriggered += Input_OnJumpTriggered;
        _input.CrouchTriggered += Input_OnCrouchTriggered;
        _input.CrouchCanceled += Input_OnCrouchCanceled;

        _interactions.InteractionExited += Interactions_OnInteractionExited;
        _interactions.ControlObjectInteracted += Interactions_OnControlObjectInteracted;
    }

    private void OnDisable()
    {
        _input.JumpTriggered -= Input_OnJumpTriggered;
        _input.CrouchTriggered -= Input_OnCrouchTriggered;
        _input.CrouchCanceled -= Input_OnCrouchCanceled;

        _interactions.InteractionExited -= Interactions_OnInteractionExited;
        _interactions.ControlObjectInteracted -= Interactions_OnControlObjectInteracted;
    }

    private void Update()
    {
        if (_followObject != null)
        {
            transform.position = _followObject.position;
            return;
        }

        if (_surfaceUpdateTimer <= 0f)
        {
            _surfaceUpdateTimer = 1f / _surfaceUpdateRate;
            UpdateSurfaceType();
        }

        if (_crouchCancellationRequested && CanStand() && !IsFrozen)
            CancelCrouch();

        HandleMovement();
        HandleLanding();

        if (_controller.isGrounded)
            _lastGroundHeight = transform.position.y;

        _surfaceUpdateTimer -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector2 input = _input.MoveInput;

        _velocity = new Vector3()
        {
            x = input.x * CurrentSpeed,
            y = _velocity.y + GRAVITY * Time.deltaTime,
            z = input.y * CurrentSpeed
        };

        if (IsFrozen)
        {
            _velocity.x = 0;
            _velocity.z = 0;
        }
  
        _controller.Move(transform.TransformDirection(_velocity) * Time.deltaTime);

        if (_controller.isGrounded)
            _velocity.y = 0f;
    }

    private void HandleLanding()
    {
        if (IsOnGround != _controller.isGrounded && _controller.isGrounded)
        {
            UpdateSurfaceType();

            float fallDistance = Mathf.Abs(_lastGroundHeight - transform.position.y);

            if (fallDistance > 0.3f)
                PlayerLanded?.Invoke();
        }

        IsOnGround = _controller.isGrounded;
    }

    private void UpdateSurfaceType()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 2f,
            _surfaceLayers, QueryTriggerInteraction.Ignore))
        {
            SurfaceType = hitInfo.transform.tag switch
            {
                Constants.TAG_STONE => SurfaceType.Stone,
                Constants.TAG_SAND => SurfaceType.Sand,
                Constants.TAG_WOOD => SurfaceType.Wood,
                Constants.TAG_METAL => SurfaceType.Metal,
                Constants.TAG_VENT => SurfaceType.Vent,
                _ => SurfaceType.Stone
            };
        }
    }

    public bool CanStand()
    {
        float sphereRadius = 0.3f;
        float castDistance = _controllerPrimaryHeight * 1.5f / 2f - sphereRadius / 2f;

        bool canStand = !Physics.SphereCast(transform.position, sphereRadius, Vector3.up,
            out var hitInfo, castDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        Debug.DrawLine(transform.position, transform.position + transform.up * castDistance, Color.red);

        return canStand;
    }

    private void Input_OnJumpTriggered()
    {
        if (_controller.isGrounded && !IsFrozen)
        {
            _velocity.y = Mathf.Sqrt(_jumpForce * -3f * GRAVITY);
            _lastGroundHeight = float.MaxValue;
        }
    }

    private void Input_OnCrouchTriggered()
    {
        if (IsFrozen)
            return;

        Vector3 center = _controllerPrimaryCenter;
        center.y -= center.y / 2f;

        _controller.center = center;
        _controller.height = _controllerPrimaryHeight / 2f;

        CurrentSpeed = _crouchSpeed;
        IsCrouching = true;

        _crouchCancellationRequested = false;
    }

    private void Input_OnCrouchCanceled() => _crouchCancellationRequested = true;

    private void CancelCrouch()
    {
        _controller.height = _controllerPrimaryHeight;
        _controller.center = _controllerPrimaryCenter;

        CurrentSpeed = _walkSpeed;
        IsCrouching = false;

        _crouchCancellationRequested = false;

        CrouchCanceled?.Invoke();
    }


    private void Interactions_OnControlObjectInteracted(InteractionControlData data)
    {
        if (data.objectToFollow != null)
        {
            CancelCrouch();
            _velocity = Vector3.zero;

            if (LeanTween.isTweening(gameObject))
                LeanTween.cancel(gameObject);

            LeanTween.move(gameObject, data.objectToFollow.position, 1f)
            .setEaseInOutQuad()
            .setOnComplete(() => 
            {
                _followObject = data.objectToFollow;
            });
        }

        if (data.freezeMovement)
            IsFrozen = true;
    }

    private void Interactions_OnInteractionExited()
    {
        _followObject = null;
        IsFrozen = false;
        _lastGroundHeight = transform.position.y;
    }
}

public enum SurfaceType
{
    Stone,
    Sand,
    Metal,
    Wood,
    Vent
}