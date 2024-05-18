using System;
using UnityEngine;

public sealed class KeyLock : ItemPlace, ILock
{
    public event Action Interacted, KeyInserted;
    public event Action<ILock> Unlocked;

    [Header("Tweening")]
    [SerializeField] private Axis _keyMovementAxis;
    [SerializeField] private float _keyOffset;
    [SerializeField] private Axis _lockRotationAxis;
    [SerializeField] private float _lockRotation;
        
    public bool IsLocked { get; private set; } = true;
    protected override void InteractInternal(InteractionData data)
    {
        if (ValidateInteraction(data.item))
        {
            if (LeanTween.isTweening(gameObject) || LeanTween.isTweening(PlacedGO))
                return;

            ProcessKeyInsertion(data);
        }
    }

    private void ProcessKeyInsertion(InteractionData data)
    {
        data.handler.Handle(this);
        Interacted?.Invoke();

        Vector3 targetKeyPosition = PlacedGO.transform.position;
        Vector3 targetLockRotation = transform.localEulerAngles + _lockRotationAxis.ToVector() * _lockRotation;

        PlacedGO.transform.position -= _keyMovementAxis.ToTransformDirection(PlacedGO.transform) * _keyOffset;
        PlacedGO.SetActive(true);

        LeanTween.move(PlacedGO, targetKeyPosition, 0.75f)
        .setEaseInOutQuad()
        .setOnComplete(() =>
        {
            LeanTween.rotateLocal(gameObject, targetLockRotation, 0.3f)
            .setEaseInOutQuad()
            .setOnComplete(() =>
            {
                Unlocked?.Invoke(this);
                IsLocked = false;
            });

            KeyInserted?.Invoke();
        });
    }
}
