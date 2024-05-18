using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObjectDetector : MonoBehaviour
{
    public event Action<GameObject> ObjectEntered, ObjectExited;

    [SerializeField] private LayerMask _mask;
    [SerializeField] private bool _handleTriggers;

    private List<GameObject> _objectsInside = new();

    public bool IsActive { get; set; }

    public bool IsDetected(GameObject gameObject)
    {
        if (IsActive)
            return _objectsInside.Contains(gameObject);
        else
            return false;
    }

    public bool IsDetectedAny()
    {
        if (IsActive)
            return _objectsInside.Count > 0;
        else
            return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger && !_handleTriggers)
            return;

        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            if (!_objectsInside.Contains(other.gameObject))
            {
                _objectsInside.Add(other.gameObject);

                if (IsActive)
                    ObjectEntered?.Invoke(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger && !_handleTriggers)
            return;

        if ((1 << other.gameObject.layer & _mask) != 0)
        {
            _objectsInside.Remove(other.gameObject);

            if (IsActive)
                ObjectExited?.Invoke(other.gameObject);
        }
    }
}