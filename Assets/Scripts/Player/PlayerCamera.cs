using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(PlayerInteractions))]
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Vector2 _sensitivity;
    [SerializeField] private float _defaultCameraHeight;
    [SerializeField] private float _crouchCameraHeight;
    [SerializeField] private Vector2 _maxViewAngles;

    private InputManager _input;
    private PlayerController _controller;
    private PlayerInteractions _interactions;

    private float _currentRotationX, _currentRotationY;
    private bool _isTweening;
    private bool _isFrozen;

    private Transform _followObject;
    private Vector2 _maxViewAnglesCurrent;

    private void Awake()
    {
        _input = ProjectContext.Instance.InputManager;
        _controller = GetComponent<PlayerController>();
        _interactions = GetComponent<PlayerInteractions>();

        _maxViewAnglesCurrent = _maxViewAngles;

        SetCameraHeight(_defaultCameraHeight);
    }

    private void OnEnable()
    {
        _input.CrouchTriggered += Input_OnCrouchTriggered;
        _controller.CrouchCanceled += Input_OnCrouchCanceled;

        _interactions.InteractionExited += Interactions_OnInteractionExited;
        _interactions.ControlObjectInteracted += Interactions_OnControlObjectInteracted;
    }

    private void OnDisable()
    {
        _input.CrouchTriggered -= Input_OnCrouchTriggered;
        _controller.CrouchCanceled -= Input_OnCrouchCanceled;

        _interactions.InteractionExited -= Interactions_OnInteractionExited;
        _interactions.ControlObjectInteracted -= Interactions_OnControlObjectInteracted;
    }

    private void Update()
    {
        if (Game.Instance.IsPaused || _isTweening || _isFrozen)
            return;

        Look(_input.MouseDelta);
    }

    private void Input_OnCrouchTriggered()
    {
        if (!_controller.IsFrozen)
            SetCameraHeight(_crouchCameraHeight);
    }

    private void Input_OnCrouchCanceled()
    {
        SetCameraHeight(_defaultCameraHeight);
    }

    private void Interactions_OnInteractionExited()
    {
        if (_isTweening)
        {
            StopAllCoroutines();
            _isTweening = false;
        }

        _followObject = null;
        _isFrozen = false;

        _maxViewAnglesCurrent = _maxViewAngles;
        _currentRotationY = _cameraTransform.eulerAngles.y;
    }

    private void Interactions_OnControlObjectInteracted(InteractionControlData data)
    {
        if (data.objectToFollow != null)
        {
            StartCoroutine(SetFollowObjectRoutine(data.objectToFollow));
        }

        _maxViewAnglesCurrent = data.lookConstraints ?? _maxViewAngles;
        _isFrozen = data.freezeRotation;
    }

    private IEnumerator SetFollowObjectRoutine(Transform followObject)
    {
        if (_isTweening)
        {
            StopAllCoroutines();
            _isTweening = false;
        }

        yield return StartCoroutine(OrientationRoutine(followObject.rotation));
        _followObject = followObject;
    }

    private void Look(Vector2 input)
    {
        _currentRotationY += input.x * Time.fixedDeltaTime * _sensitivity.x;
        _currentRotationX -= input.y * Time.fixedDeltaTime * _sensitivity.y;

        _currentRotationX = Mathf.Clamp(_currentRotationX, -_maxViewAnglesCurrent.x, _maxViewAnglesCurrent.x);
        _currentRotationY = Mathf.Clamp(_currentRotationY, -_maxViewAnglesCurrent.y, _maxViewAnglesCurrent.y);

        float newRotationX, newRotationY;
        
        if (_followObject == null)
        {
            newRotationX = _currentRotationX;
            newRotationY = _currentRotationY;
        }
        else
        {
            newRotationX = _followObject.eulerAngles.x + _currentRotationX;
            newRotationY = _followObject.eulerAngles.y + _currentRotationY; 
        }

        transform.rotation = Quaternion.Euler(0f, newRotationY, 0f);
        _cameraTransform.rotation = Quaternion.Euler(newRotationX, newRotationY, 0f);
    }

    private void SetCameraHeight(float height)
    {
        Vector3 cameraPosition = _cameraTransform.localPosition;
        cameraPosition.y = height;
        _cameraTransform.localPosition = cameraPosition;
    }

    private IEnumerator OrientationRoutine(Quaternion targetOrientation)
    {
        _isTweening = true;

        Quaternion initialOrientation = _cameraTransform.rotation;
        Quaternion initialTransformOrientation = transform.rotation;

        Quaternion targetTransformOrientation = Quaternion.Euler(transform.eulerAngles.x,
            targetOrientation.eulerAngles.y, transform.eulerAngles.z);

        float t = 0f;
        float smoothTime = 0.33f;
        float currentVelocity = 0f;

        while (t < 0.99f)
        {
            t = Mathf.SmoothDamp(t, 1f, ref currentVelocity, smoothTime);

            Quaternion currentOrientation = Quaternion.Lerp(initialOrientation, targetOrientation, t);
            Quaternion currentTransformOrientation = Quaternion.Lerp(initialTransformOrientation,
                targetTransformOrientation, t);

            transform.rotation = currentTransformOrientation;
            _cameraTransform.rotation = currentOrientation;

            yield return null;
        }

        transform.rotation = targetTransformOrientation;
        _cameraTransform.rotation = targetOrientation;

        _currentRotationX = 0;
        _currentRotationY = 0;

        _isTweening = false;
    }
}
