using UnityEngine;

public class FanVisuals : MonoBehaviour
{
    [SerializeField] private float _maxSpeed;
    [SerializeField] private Axis _axis;
    [SerializeField] private Transform[] _propellers;
    [SerializeField] private bool _activeByDefault;

    private float _currentSpeed;
    private float _targetSpeed;

    private bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            _targetSpeed = _isActive ? _maxSpeed : 0f;
        }
    }

    private void Awake()
    {
        IsActive = _activeByDefault;

        if (_propellers.Length == 0)
            return;

        foreach (var p in _propellers)
        {
            float angleOffset = Random.Range(-360f, 360f);
            p.Rotate(_axis.ToVector() * angleOffset, Space.Self);
        }
    }

    private void Update()
    {
        _currentSpeed = Mathf.Lerp(_currentSpeed, _targetSpeed, Time.deltaTime);
        UpdatePropellers();
    }

    private void UpdatePropellers()
    {
        if (_propellers.Length == 0 || _currentSpeed < 0.01f)
            return;

        for (int i = 0; i < _propellers.Length; i++)
            _propellers[i].Rotate(_axis.ToVector() * _currentSpeed * Time.deltaTime, Space.Self);
    }
}
