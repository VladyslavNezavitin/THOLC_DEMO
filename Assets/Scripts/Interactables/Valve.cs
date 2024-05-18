using System;
using System.Collections;
using UnityEngine;

public class Valve : Interactable
{
    public event Action RotationStarted, RotationStopped;

    [SerializeField] private float _rotationSpeed = 120f;
    [SerializeField] private float _minAngle = 0f;
    [SerializeField] private float _maxAngle = 359f;
    [SerializeField] private Axis _axis = Axis.X;

    private float _currentAngle;
    private bool _negativeRotation;

    private void Awake() => _currentAngle = _axis.GetLocalRotationAngle(transform); 

    protected override void InteractInternal(InteractionData data)
    {
        StartCoroutine(RotationRoutine(data));
    }

    private IEnumerator RotationRoutine(InteractionData data)
    {
        RotationStarted?.Invoke();

        while (data.isHoldingInteraction)
        {
            if ((_currentAngle < _maxAngle || _negativeRotation) &&
                (_currentAngle > _minAngle || !_negativeRotation))
            {
                float delta = _rotationSpeed * Time.deltaTime * (_negativeRotation ? -1f : 1f);
                transform.Rotate(_axis.ToVector(), delta, Space.Self);
                _currentAngle += delta;
            }
            else
            {
                data.forceExitCallback?.Invoke();
                break;
            }
            
            yield return null;
        }

        _negativeRotation = !_negativeRotation;
        RotationStopped?.Invoke();
    }
}
