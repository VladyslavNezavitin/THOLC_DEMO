using System;
using UnityEngine;

public class GateControlPanel : MonoBehaviour
{
    public event Action OpenButtonPressed, CloseButtonPressed;

    [SerializeField] private Button _openButton;
    [SerializeField] private Button _closeButton;

    private void OnEnable()
    {
        _openButton.Pressed += OpenButton_OnPressed;
        _closeButton.Pressed += CloseButton_OnPressed;
    }

    private void OnDisable()
    {
        _openButton.Pressed -= OpenButton_OnPressed;
        _closeButton.Pressed -= CloseButton_OnPressed;
    }

    private void OpenButton_OnPressed(Button button)
    {
        OpenButtonPressed?.Invoke();
    }
    
    private void CloseButton_OnPressed(Button button)
    {
        CloseButtonPressed?.Invoke();
    }
}
