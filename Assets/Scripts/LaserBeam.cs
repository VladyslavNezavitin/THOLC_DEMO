using System;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public event Action<Transform> PlayerEntered;
    private Vector3 _initialScale;

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            if (_isActive)
                transform.localScale = _initialScale;
            else
                transform.localScale = Vector3.zero;
        }
    }

    private void Awake()
    {
        _initialScale = transform.localScale;   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Constants.LAYER_PLAYER)
        {
            PlayerEntered?.Invoke(other.gameObject.transform);
        }
    }
}
