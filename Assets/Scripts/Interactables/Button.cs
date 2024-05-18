using System;
using UnityEngine;

public sealed class Button : Interactable
{
    public event Action<Button> Pressed;

    [SerializeField] private char _value = ' ';

    [Space, Header("Tweening")]
    [SerializeField] private float _pressDuration = 0.3f;
    [SerializeField] private float _pressAmount = 0.01f;
    [SerializeField] private Axis _pressAxis = Axis.X;

    public char Value => _value;

    protected override void InteractInternal(InteractionData data)
    {
        if (LeanTween.isTweening(gameObject))
            return;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + _pressAxis.ToTransformDirection(transform) * _pressAmount;

        LeanTween.move(gameObject, targetPosition, _pressDuration / 2f)
        .setEase(LeanTweenType.easeInOutSine)
        .setOnComplete(() =>
        {
            LeanTween.move(gameObject, startPosition, _pressDuration / 2f)
                .setEase(LeanTweenType.easeInOutSine);
        });

        Pressed?.Invoke(this);
    }
}
