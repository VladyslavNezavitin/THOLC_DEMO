using System;
using UnityEngine;

public sealed class CashRegisterStorageController : MonoBehaviour, IInteractableController
{
    [SerializeField] private CashRegister _cashRegister;

    private IOpenable _openable;
    public bool CanInteract => false;

    private void OnEnable()
    {
        _cashRegister.StorageOpened += CashRegister_OnStorageOpened;
    }

    private void OnDisable()
    {
        _cashRegister.StorageOpened -= CashRegister_OnStorageOpened;
    }

    private void CashRegister_OnStorageOpened() => _openable?.Open();

    void IInteractableController.Initialize(IOpenable openable, IClosable closable)
    {
        if (openable == null)
            throw new ArgumentException("IOpenable cannot be null for this controller!");

        _openable = openable;
    }
}