using System;

public class Keypad
{
    public event Action Succeed, Failed, InputChanged;

    private readonly string _code;

    private string _input;
    public string Input
    {
        get => _input;
        set
        {
            if (_input.Length <= _code.Length)
            {
                _input = value;
                InputChanged?.Invoke();
            }
        }
    }

    public Keypad(string code)
    {
        _code = code;
        _input = string.Empty;
    }

    public void Clear() => Input = string.Empty;

    public bool TryEnter(string input)
    {
        if (input == _code)
            Succeed?.Invoke();
        else
            Failed?.Invoke();

        Clear();
        return input == _code;
    }
}
