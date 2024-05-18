using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(KeypadBehaviour))]
public sealed class KeypadDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro _displayText;

    [SerializeField] private string _messageSuccess = "Success";
    [SerializeField] private string _messageFail = "Fail";
    [SerializeField] private float _resultDisplayTime = 1f;

    [SerializeField, ColorUsage(true, true)] private Color _colorDefault = Color.white * Mathf.Pow(2f, 2f);
    [SerializeField, ColorUsage(true, true)] private Color _colorSuccess = Color.green * Mathf.Pow(2f, 2f);
    [SerializeField, ColorUsage(true, true)] private Color _colorFail = Color.red * Mathf.Pow(2f, 2f);

    private KeypadBehaviour _keypad;
    private bool _displayingResult;

    private void OnEnable()
    {
        _keypad = GetComponent<KeypadBehaviour>();
        _keypad.Base.Succeed += Keypad_OnSucceed;
        _keypad.Base.Failed += Keypad_OnFailed;
        _keypad.Base.InputChanged += Keypad_OnInputChanged;
    }

    private void OnDisable()
    {
        _keypad.Base.Succeed -= Keypad_OnSucceed;
        _keypad.Base.Failed -= Keypad_OnFailed;
        _keypad.Base.InputChanged -= Keypad_OnInputChanged;
    }

    private void Keypad_OnFailed()
    {
        if (!_displayingResult)
            StartCoroutine(DisplayResultRoutine(_messageFail, _colorFail));
    }

    private void Keypad_OnSucceed()
    {
        if (!_displayingResult)
            StartCoroutine(DisplayResultRoutine(_messageSuccess, _colorSuccess));
    }

    private void Keypad_OnInputChanged()
    {
        if (!_displayingResult)
            UpdateDisplayText(_keypad.Base.Input, _colorDefault);
    }

    private IEnumerator DisplayResultRoutine(string result, Color color)
    {
        UpdateDisplayText(result, color);
        _displayingResult = true;

        yield return new WaitForSeconds(_resultDisplayTime);

        _displayingResult = false;
        UpdateDisplayText(_keypad.Base.Input, _colorDefault);
    }

    private void UpdateDisplayText(string text, Color color)
    {
        _displayText.text = text;
        _displayText.faceColor = color;
    }
}