using UnityEngine;

public class KeypadSound : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private SoundPlaybackData _keyPressedSound;
    [SerializeField] private SoundPlaybackData _successSound;
    [SerializeField] private SoundPlaybackData _failSound;
    [SerializeField] private SoundPlaybackData _cardInsertionSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private KeypadBehaviour _keypad;
    private KeypadCardReader _cardReader;


    private void Awake()
    {
        _keypad = GetComponent<KeypadBehaviour>();
        _cardReader = GetComponent<KeypadCardReader>();
    }

    private void OnEnable()
    {
        if (_keypad != null)
        {
            _keypad.Base.Succeed += Keypad_OnSucceed;
            _keypad.Base.Failed += Keypad_OnFailed;
            _keypad.ButtonPressed += Keypad_OnButtonPressed;
        }

        if (_cardReader != null)
        {
            _cardReader.CardInserted += CardReader_OnCardInserted;
        }
    }

    private void OnDisable()
    {
        if (_keypad != null)
        {
            _keypad.Base.Succeed -= Keypad_OnSucceed;
            _keypad.Base.Failed -= Keypad_OnFailed;
            _keypad.ButtonPressed -= Keypad_OnButtonPressed;
        }

        if (_cardReader != null)
        {
            _cardReader.CardInserted -= CardReader_OnCardInserted;
        }
    }

    private void Keypad_OnSucceed() =>
        SoundManager.PlaySound(_successSound, transform.position);

    private void Keypad_OnFailed() =>
        SoundManager.PlaySound(_failSound, transform.position);

    private void CardReader_OnCardInserted() =>
        SoundManager.PlaySound(_cardInsertionSound, transform.position);

    private void Keypad_OnButtonPressed(Button button) =>
        SoundManager.PlaySound(_keyPressedSound, transform.position);
}