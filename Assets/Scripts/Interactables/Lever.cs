using System;
using System.Collections;
using UnityEngine;

public sealed class Lever : Interactable
{
    public event Action Pulled, AnimationStopped;

    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _maxAngle = 90f;
    [SerializeField] private Axis _axis;
    private bool _isRotating;

    protected override void InteractInternal(InteractionData data)
    {
        if (!_isRotating)
        {
            StartCoroutine(RotationRoutine());
            Pulled?.Invoke();
        }
    }

    private IEnumerator RotationRoutine()
    {
        Vector3 initialOrientation = transform.localEulerAngles;
        Vector3 currentOrientation = initialOrientation;
        float sin, t = 0f;

        _isRotating = true;

        while (t < 1f)
        {
            sin = Mathf.Sin(t * Mathf.PI);
            t += Time.deltaTime / _animationDuration;

            switch (_axis)
            {
                case Axis.X: currentOrientation.x = initialOrientation.x + sin * _maxAngle; break;
                case Axis.Y: currentOrientation.y = initialOrientation.y + sin * _maxAngle; break;
                case Axis.Z: currentOrientation.z = initialOrientation.z + sin * _maxAngle; break;
            }

            transform.localEulerAngles = currentOrientation;
            yield return null;
        }


        transform.localEulerAngles = initialOrientation;
        _isRotating = false;

        AnimationStopped?.Invoke();
    }
}
