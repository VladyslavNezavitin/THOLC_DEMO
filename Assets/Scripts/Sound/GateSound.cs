using UnityEngine;

[RequireComponent(typeof(GateController))]
public class GateSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _openingSound;
    [SerializeField] private SoundPlaybackData _closingSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private GateController _controller;

    private void Awake() => _controller = GetComponent<GateController>();
    private void OnEnable()
    {
        _controller.OpenTriggered += Controller_OnOpenTriggered;
        _controller.CloseTriggered += Controller_OnCloseTriggered;
    }

    private void OnDisable()
    {
        _controller.OpenTriggered -= Controller_OnOpenTriggered;
        _controller.CloseTriggered -= Controller_OnCloseTriggered;
    }

    private void Controller_OnCloseTriggered() =>
        SoundManager.PlaySound(_closingSound, transform.position);

    private void Controller_OnOpenTriggered() =>
        SoundManager.PlaySound(_openingSound, transform.position);
}
