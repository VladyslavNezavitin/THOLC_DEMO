using UnityEngine;

public class PowerControllerSound : MonoBehaviour
{
    [SerializeField] private SoundPlaybackData _powerOnSound;
    [SerializeField] private SoundPlaybackData _powerOffSound;

    private SoundManager SoundManager => ProjectContext.Instance.SoundManager;
    private ElectricalPanel _controller;

    private void Awake() => _controller = GetComponent<ElectricalPanel>();
    
    private void OnEnable()
    {
        _controller.PowerOn += Panel_OnPowerOn;
        _controller.PowerOff += Panel_OnPowerOff;   
    }

    private void OnDisable()
    {
        _controller.PowerOn -= Panel_OnPowerOn;
        _controller.PowerOff -= Panel_OnPowerOff;
    }
    
    private void Panel_OnPowerOn() =>
        SoundManager.PlaySound(_powerOnSound, transform.position);

    private void Panel_OnPowerOff() =>
        SoundManager.PlaySound(_powerOffSound, transform.position);
}
