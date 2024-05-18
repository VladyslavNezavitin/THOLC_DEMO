using System;
using UnityEngine;

[RequireComponent(typeof(KeypadBehaviour))]
public sealed class CashRegister : MonoBehaviour
{
    public event Action StorageOpened;

    [SerializeField] private Lever _lever;
    [SerializeField] private Drawer _storage;

    private KeypadBehaviour _keypad;

    private void Awake()
    {
        _keypad = GetComponent<KeypadBehaviour>();
    }

    private void OnEnable()
    {
        _lever.Pulled += Lever_OnPulled;
        _storage.Opened += Storage_OnOpened;
    }

    private void OnDisable()
    {
        _lever.Pulled -= Lever_OnPulled;
        _storage.Opened -= Storage_OnOpened;
    }

    private void Lever_OnPulled()
    {
        if (_keypad.Base.TryEnter(_keypad.Base.Input))
            OpenStorage();
    }

    private void OpenStorage() => StorageOpened?.Invoke();

    private void Storage_OnOpened()
    {
        GameObject storageGO = _storage.gameObject;
        Destroy(_storage);

        storageGO.AddComponent<Examinable>();
    }
}
