using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(TrolleyController))]
public class Trolley : Interactable, IControlInteractable
{
    public event Action Interacted, Exited, ObjectHit;

    [SerializeField] private ObjectDetector _playerDetector;
    [SerializeField] private ObjectDetector _collisionDetector;
    [SerializeField] private Transform _playerPoint;
    [SerializeField] private Collider[] _boundsColliders;

    [Space, Header("Handle")]
    [SerializeField] private Transform _handle;
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Axis _tiltAxis = Axis.X;
    [SerializeField] private float _tiltAngle = 25f;
    
    private bool _isTilting;
    private Quaternion _initialHandleRotation;
    private TrolleyController _controller;
    private Rigidbody _rb;

    public bool IsActive { get; private set; }
    public float CurrentSpeed => _rb.velocity.magnitude;

    public InteractionControlData GetControlData() => new InteractionControlData()
    {
        objectToFollow = _playerPoint,
        lookConstraints = new Vector2(45, 45),
        freezeMovement = true,
        freezeItem = true
    };

    private void Awake()
    {
        _controller = GetComponent<TrolleyController>();
        _rb = GetComponent<Rigidbody>();
        _initialHandleRotation = _handle.localRotation;

        _playerDetector.IsActive = true;

        Deactivate();
    }

    private void OnEnable() => _collisionDetector.ObjectEntered += Collision_ObjectEntered;
    private void OnDisable() => _collisionDetector.ObjectEntered -= Collision_ObjectEntered;
    private void Collision_ObjectEntered(GameObject gameObject) => ObjectHit?.Invoke();

    protected override bool ValidateInteractionInternal(Item item) => _playerDetector.IsDetectedAny() && !IsActive;
    protected override void InteractInternal(InteractionData data)
    {
        if (_playerDetector.IsDetectedAny() && !IsActive)
        {
            if (_isTilting)
                StopAllCoroutines();

            StartCoroutine(TrolleyMovementSetup(data));

            data.handler.Handle(this);

            Interacted?.Invoke();
        }
    }

    private IEnumerator TrolleyMovementSetup(InteractionData data)
    {
        Quaternion handleRotation = _initialHandleRotation *
                Quaternion.AngleAxis(_tiltAngle, _tiltAxis.ToVector());

        yield return StartCoroutine(HandleRotationRoutine(handleRotation));

        StartCoroutine(TrolleyMovementRoutine(data));
    }

    private IEnumerator TrolleyMovementRoutine(InteractionData data)
    {
        Activate();

        while (!data.isExitRequested || _isTilting)
        {
            _controller.HandleMovement(data.input);
            yield return new WaitForFixedUpdate();
        }

        StartCoroutine(HandleRotationRoutine(_initialHandleRotation));
        Deactivate();
        
        Exited?.Invoke();
    }

    private IEnumerator HandleRotationRoutine(Quaternion targetRotation)
    {
        _isTilting = true;
        Quaternion currentRotation = _handle.localRotation;
        
        float t = 0f;
        float currentVelocity = 0f;
        float smoothTime = _animationDuration / 3.33f;

        while (t < 0.99f)
        {
            t = Mathf.SmoothDamp(t, 1f, ref currentVelocity, smoothTime);

            _handle.localRotation = Quaternion.Lerp(currentRotation, targetRotation, t);
            yield return null;
        }

        _handle.localRotation = targetRotation;
        _isTilting = false;
    }

    private void Activate()
    {
        gameObject.SetLayer(Constants.LAYER_PLAYER);
        _rb.isKinematic = false;
        _collisionDetector.IsActive = true;

        foreach (var collider in _boundsColliders)
            collider.enabled = true;

        IsActive = true;
    }

    private void Deactivate()
    {
        gameObject.SetLayer(Constants.LAYER_DEFAULT);
        _handle.gameObject.SetLayer(Constants.LAYER_DEFAULT);
        _rb.isKinematic = true;
        _collisionDetector.IsActive = false;

        foreach (var collider in _boundsColliders)
            collider.enabled = false;

        IsActive = false;
    }

}