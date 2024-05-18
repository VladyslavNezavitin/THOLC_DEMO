using System;
using UnityEngine;

public interface IPowerController
{
    public event Action PowerOn, PowerOff;
}

public class LaserController : MonoBehaviour
{
    public event Action<Transform> PlayerDetected;

    [SerializeField] private LaserBeam[] _lasers;
    [SerializeField] private GameObject _controllerGO;
    private IPowerController _controller;

    private void OnValidate()
    {
        if (_controllerGO == null)
            return;

        _controller = _controllerGO.GetComponent<IPowerController>();

        if (_controller == null)
            throw new InvalidOperationException($"GameObject {_controllerGO.name} does not have ElectricalPanel component!");
    }

    private void OnEnable()
    {
        if (_controller != null)
        {
            _controller.PowerOn += Controller_OnPowerOn;
            _controller.PowerOff += Controller_OnPowerOff;
        }

        foreach (var laser in _lasers)
            laser.PlayerEntered += Laser_OnPlayerEntered;
    }

    private void OnDisable()
    {
        if (_controller != null)
        {
            _controller.PowerOff -= Controller_OnPowerOff;
            _controller.PowerOn -= Controller_OnPowerOn;
        }

        foreach (var laser in _lasers)
            laser.PlayerEntered -= Laser_OnPlayerEntered;
    }

    private void Start()
    {
        Controller_OnPowerOff();
        Controller_OnPowerOn();
    }

    private void Controller_OnPowerOn()
    {
        foreach (var laser in _lasers)
            laser.IsActive = true;
    }

    private void Controller_OnPowerOff()
    {
        foreach (var laser in _lasers)
            laser.IsActive = false;
    }

    private void Laser_OnPlayerEntered(Transform playerTransform)
    {
        PlayerDetected?.Invoke(playerTransform);
    }
}
