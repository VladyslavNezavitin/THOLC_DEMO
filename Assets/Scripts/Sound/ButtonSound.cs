using UnityEngine;

[RequireComponent(typeof(Button))]
public class ButtonSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _pressSound;
    private Button _button;
    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;

    private void Awake() => _button = GetComponent<Button>();
    private void OnEnable() => _button.Pressed += Button_OnPressed;
    private void OnDisable() => _button.Pressed -= Button_OnPressed;

    private void Button_OnPressed(Button button) =>
        SoundManager.PlaySound(_pressSound, transform.position);
}
