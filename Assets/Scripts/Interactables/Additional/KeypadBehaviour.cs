using System;
using UnityEngine;

public class KeypadBehaviour : MonoBehaviour
{
    public event Action<Button> ButtonPressed;

    [SerializeField] private string _code;
    [SerializeField] private Button[] _buttons;

    private Keypad _base;
    public Keypad Base
    {
        get
        {
            if (_base == null)
                _base = new Keypad(_code);

            return _base;
        }
    }

    private void OnEnable()
    {
        foreach (var button in _buttons)
            button.Pressed += Button_OnPressed;
    }

    private void OnDisable()
    {
        foreach (var button in _buttons)
            button.Pressed -= Button_OnPressed;
    }

    private void Button_OnPressed(Button button)
    {
        ButtonPressed?.Invoke(button);

        switch (button.Value)
        {
            case ' ': return;
            case 'E': Base.TryEnter(Base.Input); return;
            case 'C': Base.Clear(); return;
        }

        if (Base.Input.Length < _code.Length)
            Base.Input += button.Value;
    }
}