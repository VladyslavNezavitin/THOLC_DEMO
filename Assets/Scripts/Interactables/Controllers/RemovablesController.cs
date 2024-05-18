using System;
using System.Collections.Generic;
using UnityEngine;

public class RemovablesController : MonoBehaviour, IInteractableController
{
    [SerializeField] private List<Removable> _removables;
    [SerializeField] private bool _openOnAllRemoved;

    private IOpenable _openable;

    public bool CanInteract => _removables.Count == 0;

    private void OnEnable()
    {
        foreach (var removable in _removables)
        {
            removable.Removed += Removable_OnRemoved;
        }
    }

    private void OnDisable()
    {
        foreach (var removable in _removables)
        {
            removable.Removed -= Removable_OnRemoved;
        }
    }

    void IInteractableController.Initialize(IOpenable openable, IClosable closable)
    {
        if (openable == null)
            throw new ArgumentException("IOpenable cannot be null for this controller!");

        _openable = openable;
    }

    private void Removable_OnRemoved(Removable removable)
    {
        _removables.Remove(removable);

        if (_removables.Count == 0)
        {
            if (_openOnAllRemoved)
                _openable.Open();
        }
    }
}
