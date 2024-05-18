using System;
using System.Collections;
using UnityEngine;

public sealed class Handle : Interactable
{
    public event Action<Handle> CurrentStepChanged;

    [SerializeField] private int _steps = 3;
    [SerializeField] private int _initialStep;
    [SerializeField] private float _stepAngle = 90f;
    [SerializeField] private float _animationDuration = .3f;
    [SerializeField] private Axis _axis = Axis.X;

    public int CurrentStep { get; private set; }

    private Quaternion _initialRotation;
    private bool _isSwitching;

    private void Awake()
    {
        _initialRotation = Quaternion.identity;
        transform.localEulerAngles = _axis.ToVector() * _initialStep * _stepAngle;

        CurrentStep = _initialStep;
    }

    protected override void InteractInternal(InteractionData data)
    {
        if (!_isSwitching)
            StartCoroutine(SwitchRoutine());
    }

    private IEnumerator SwitchRoutine()
    {
        _isSwitching = true;
        Quaternion currentRotation = transform.localRotation;
        Quaternion targetRotation;

        Vector3 axis = _axis.ToVector();

        if (CurrentStep < _steps)
        {
            targetRotation = currentRotation * Quaternion.AngleAxis(_stepAngle, axis);
            CurrentStep++;
        }
        else
        {
            targetRotation = _initialRotation;
            CurrentStep = 0;
        }

        float t = 0f;
        float currentVelocity = 0f;
        float smoothTime = _animationDuration / 3.33f;

        while (t < 0.99f)
        {
            t = Mathf.SmoothDamp(t, 1f, ref currentVelocity, smoothTime);

            transform.localRotation = Quaternion.Lerp(currentRotation, targetRotation, t);
            yield return null;
        }

        transform.localRotation = targetRotation;
        _isSwitching = false;

        CurrentStepChanged?.Invoke(this);
    }
}