using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Removable : Interactable
{
    public event Action<Removable> Removed;
    public event Action<float> ObjectHit;

    [SerializeField] private Item.ID _requiredItemID;

    private Rigidbody _rb;
    private float _lastGroundHeight;

    public bool IsRemoved { get; private set; }

    private void Awake() => _rb = GetComponent<Rigidbody>();

    protected override bool ValidateInteractionInternal(Item item) => 
        item != null && item.Id == _requiredItemID && !IsRemoved;

    protected override void InteractInternal(InteractionData data)
    {
        if (ValidateInteractionInternal(data.item))
        {
            IsRemoved = true;
            _lastGroundHeight = transform.position.y;

            Removed?.Invoke(this);
            StartCoroutine(RemovableProcessRoutine());
        }
    }

    private IEnumerator RemovableProcessRoutine()
    {
        yield return StartCoroutine(Routines.RigidbodyFallRoutine(_rb));
        _rb.detectCollisions = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fallDistance = Mathf.Abs(_lastGroundHeight - transform.position.y);

        if (fallDistance > 0.1f)
        {
            ObjectHit?.Invoke(fallDistance);
        }

        _lastGroundHeight = transform.position.y;
    }
}