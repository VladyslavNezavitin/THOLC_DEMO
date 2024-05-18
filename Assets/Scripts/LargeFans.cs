using UnityEngine;

public class LargeFans : MonoBehaviour
{
    [SerializeField] private FanVisuals[] _fans;
    [SerializeField] private Switch _controlSwitch;

    private void OnEnable()
    {
        _controlSwitch.TurnedOn += Switch_OnTurnedOn;
        _controlSwitch.TurnedOff += Switch_OnTurnedOff;
    }

    private void OnDisable()
    {
        _controlSwitch.TurnedOn -= Switch_OnTurnedOn;
        _controlSwitch.TurnedOff -= Switch_OnTurnedOff;
    }

    private void Switch_OnTurnedOn()
    {
        foreach (var fan in _fans)
            fan.IsActive = true;
    }

    private void Switch_OnTurnedOff()
    {
        foreach (var fan in _fans)
            fan.IsActive = false;
    }
}
