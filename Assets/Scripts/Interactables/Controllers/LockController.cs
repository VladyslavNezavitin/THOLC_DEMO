using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class LockController : MonoBehaviour, IInteractableController
{
    [SerializeField] private List<GameObject> _lockObjects;
    [SerializeField] private bool _openOnUnlocked;
    [SerializeField] private bool _openOnAnyUnlocked;

    private ILock[] _locks;
    private IOpenable _openable;

    public bool CanInteract => !IsLocked();
    void IInteractableController.Initialize(IOpenable openable, IClosable closable)
    {
        if (openable == null)
            throw new ArgumentException("IOpenable cannot be null for this controller!");

        _openable = openable;
    }

    private void OnEnable()
    {
        foreach (var l in _locks)
            l.Unlocked += Lock_OnUnlocked;
    }

    private void OnDisable()
    {
        foreach (var l in _locks)
            l.Unlocked -= Lock_OnUnlocked;
    }

    private void OnValidate()
    {
        if (_lockObjects == null)
            return;

        List<ILock> locks = new List<ILock>();

        foreach (var lo in _lockObjects)
        {
            if (lo == null)
                continue;

            if (lo.TryGetComponent<ILock>(out var l))
                locks.Add(l);
            else
                throw new ArgumentException("Objects in this list must inherit ILock interface!");
        }
        
        _locks = locks.ToArray();
    }

    private void Lock_OnUnlocked(ILock l)
    {
        if (!IsLocked() && _openOnUnlocked)
            _openable.Open();
    }

    private bool IsLocked()
    {
        int unlocked = 0;

        foreach (var l in _locks)
        {
            if (!l.IsLocked)
            {
                if (_openOnAnyUnlocked)
                    return false;

                unlocked++;
            }
        }

        return unlocked < _locks.Length;
    }
}

public interface ILock
{
    public event Action<ILock> Unlocked;
    public bool IsLocked { get; }
}