using UnityEngine;

public class DemoPlayerTeleporter : MonoBehaviour
{
    [SerializeField] private Transform _teleportationPoint;

    private LaserController _laserController;
    private void Awake()
    {
        _laserController = GetComponent<LaserController>();
    }

    private void OnEnable()
    {
        _laserController.PlayerDetected += Laser_OnPlayerDetected;
    }

    private void OnDisable()
    {
        _laserController.PlayerDetected -= Laser_OnPlayerDetected;

    }

    private void Laser_OnPlayerDetected(Transform transform)
    {
        transform.position = _teleportationPoint.position;
    }
}
