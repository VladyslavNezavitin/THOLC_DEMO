using System;
using UnityEngine;

public class TrolleyController : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 100f;
    [SerializeField] private float _rotationSpeed = 75f;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void HandleMovement(Vector2 input)
    {
        float rotationFactor = Mathf.Abs(_rb.velocity.magnitude / (_maxSpeed * Time.fixedDeltaTime));
        Quaternion rotationDelta = Quaternion.Euler(transform.up * input.x *
            rotationFactor * _rotationSpeed * Time.fixedDeltaTime);

        _rb.MoveRotation(_rb.rotation * rotationDelta);

        Vector3 targetVelocity = transform.forward * input.y * _maxSpeed * Time.fixedDeltaTime;
        targetVelocity.y = _rb.velocity.y;

        _rb.velocity = targetVelocity;
    }
}
