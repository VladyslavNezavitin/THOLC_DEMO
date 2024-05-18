using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Item : Interactable
{
    public event Action Picked;
    public event Action<float> ObjectHit;

    private Rigidbody _rb;
    private float _lastGroundHeight;

    public enum ID
    {
        Undefined = 0,
        Valve,
        Fuse,
        Keycard_Turnstile = 50,
        Keycard_Freezer,
        Key_Storage = 100,
        Key_Roof,
        Key_StorageElPanel,
        Wrench = 150
    }

    [SerializeField] private ID _id;
    public ID Id => _id;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.excludeLayers += 1 << Constants.LAYER_PLAYER;
        _rb.isKinematic = true;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    protected override void InteractInternal(InteractionData data)
    {
        data.handler.Handle(this);
    }

    public void Pick()
    {
        _rb.isKinematic = true;

        gameObject.SetLayer(Constants.LAYER_RENDER_ON_TOP);
        Picked?.Invoke();
    }

    public void Drop()
    {
        gameObject.SetLayer(Constants.LAYER_DEFAULT);
        _lastGroundHeight = transform.position.y;

        StartCoroutine(Routines.RigidbodyFallRoutine(_rb));
    }

    private void OnCollisionEnter(Collision collision)
    {
        float fallDistance = Mathf.Abs(_lastGroundHeight - transform.position.y);

        if (fallDistance > 0.1f)
        {
            ObjectHit?.Invoke(fallDistance);
            _lastGroundHeight = transform.position.y;
        }
    }
}