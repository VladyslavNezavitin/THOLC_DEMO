using System;
using UnityEngine;

public sealed class KeypadButtonController : MonoBehaviour, IInteractableController
{
    [SerializeField] private KeypadBehaviour _keypad;
    [SerializeField] private Button _button;

    private IOpenable _openable;
    private IClosable _closable;

    public bool CanInteract => false;
    void IInteractableController.Initialize(IOpenable openable, IClosable closable)
    {
        if (openable == null)
            throw new ArgumentException("IOpenable and IClosable cannot be null for this controller!");

        _openable = openable;
        _closable = closable;
    }

    private void OnEnable()
    {
        _keypad.Base.Succeed += Keypad_OnSucceed;
        _button.Pressed += Button_OnPressed;
    }

    private void OnDisable()
    {
        _keypad.Base.Succeed -= Keypad_OnSucceed;
        _button.Pressed -= Button_OnPressed;
    }

    private void Button_OnPressed(Button button) => Toggle();
    private void Keypad_OnSucceed() => Toggle();

    private void Toggle()
    {
        if (_openable.IsOpen)
            _closable.Close();
        else
            _openable.Open();
    }
}