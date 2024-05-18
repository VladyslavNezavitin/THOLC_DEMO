using System;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public event Action OpenTriggered, CloseTriggered;

    [SerializeField] private GateControlPanel _panel;
    [SerializeField] private BoxCollider _shuttersCollider;
    private Animator _animator;

    private readonly int closeStateHash = Animator.StringToHash("Gate_02_Close");
    private readonly int openTriggerHash = Animator.StringToHash("Open");
    private readonly int closeTriggerHash = Animator.StringToHash("Close");
    
    private bool _isCooldown;
    private bool _isOpen;

    private void Awake() => _animator = GetComponent<Animator>();

    private void OnEnable()
    {
        _panel.OpenButtonPressed += Panel_OnOpenButtonPressed;
        _panel.CloseButtonPressed += Panel_OnCloseButtonPressed;

        _animator.Play(closeStateHash, 0, 1f);
    }

    private void OnDisable()
    {
        _panel.OpenButtonPressed -= Panel_OnOpenButtonPressed;
        _panel.CloseButtonPressed -= Panel_OnCloseButtonPressed;
    }

    private void Panel_OnOpenButtonPressed()
    {
        if (!_isCooldown && !_isOpen)
        {
            _animator.SetTrigger(openTriggerHash);
            _isOpen = true;
            OpenTriggered?.Invoke();
        }
    }

    private void Panel_OnCloseButtonPressed()
    {
        if (!_isCooldown && _isOpen)
        {
            _animator.SetTrigger(closeTriggerHash);
            _isOpen = false;
            CloseTriggered?.Invoke();
        }
    }

    public void OnAnimationFinished() => _isCooldown = false;
    public void OnAnimationStarted() => _isCooldown = true;
    public void ToggleCollider() => _shuttersCollider.enabled = !_isOpen;
}