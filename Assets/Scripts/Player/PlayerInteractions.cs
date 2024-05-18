using System;
using UnityEngine;

public interface IInteractionHandler
{
    void Handle(ItemPlace place);
    void Handle(Item item);
    void Handle(IControlInteractable controlObject);
}

[RequireComponent(typeof(PlayerItem))]
public class PlayerInteractions : MonoBehaviour, IInteractionHandler
{
    public event Action<InteractionFeedback> InteractableDetected;
    public event Action<InteractionControlData> ControlObjectInteracted;
    public event Action InteractionExited;

    [SerializeField] private Transform _raycastOrigin;
    [SerializeField] private float _rayMaxDistance;
    [Range(0, 100), SerializeField] private int _detectionRate;
    [SerializeField] private LayerMask _detectionMask; 
    [SerializeField] private Transform _examinationPoint;

    private InputManager _input;
    private PlayerItem _itemController;

    private InteractionData _currentInteractionData;
    private InteractionControlData _currentInteractionControlData;
    private float _detectionTimer;


    private Interactable _detectedInteractable;
    private Interactable DetectedInteractable
    {
        get => _detectedInteractable;
        set
        {
            InteractableDetected?.Invoke(value?.GetFeedback(_itemController.Item));

            if (_detectedInteractable != value)
            {
                _detectedInteractable = value;

                if (_currentInteractionData != null)
                    _currentInteractionData.isHoldingInteraction = false;
            }
        }
    }

    private void Awake()
    {
        _input = ProjectContext.Instance.InputManager;
        _itemController = GetComponent<PlayerItem>();
    }

    private void OnEnable()
    {
        _input.InteractionTriggered += Input_OnInteractionTriggered;
        _input.InteractionCanceled += Input_OnInteractionCanceled;
        //_input.EscapeTriggered += Input_EscapeTriggered;
    }

    private void OnDisable()
    {
        _input.InteractionTriggered -= Input_OnInteractionTriggered;
        _input.InteractionCanceled -= Input_OnInteractionCanceled;
        //_input.EscapeTriggered -= Input_EscapeTriggered;
    }

    private void Update()
    {
        if (_detectionTimer <= 0)
        {
             DetectInteractables();
            _detectionTimer = 1f / _detectionRate;
        }

        _detectionTimer -= Time.deltaTime;

        if (_currentInteractionData != null)
        {
            if (LeanTween.isTweening(gameObject))
            {
                _currentInteractionData.input = Vector2.zero;
                _currentInteractionData.inputRaw = Vector2.zero;
            }
            else
            {
                _currentInteractionData.input = _input.MoveInput;
                _currentInteractionData.inputRaw = _input.MoveInputRAW;
            }
        }
    }

    private void DetectInteractables()
    {
        if (PerformRaycast(out var hitInfo))
            DetectedInteractable = hitInfo.transform.GetComponent<Interactable>();
        else
            DetectedInteractable = null;
    }

    private void Input_OnInteractionTriggered()
    {
        if (!TryExitInteraction())
            return;

        if (PerformRaycast(out var hitInfo))
        {
            if (hitInfo.transform.TryGetComponent<Interactable>(out var interactable))
            {
                /*if (_currentInteractionData != null)
                {
                    _currentInteractionData.isExitRequested = true;
                    _currentInteractionData.isHoldingInteraction = false;
                }*/

                _currentInteractionData = new InteractionData()
                {
                    handler = this,
                    examinationPoint = _examinationPoint,
                    isHoldingInteraction = true,
                    item = _itemController.Item,
                    playerPosition = transform.position,
                    forceExitCallback = ExitInteraction
                };

                interactable.Interact(_currentInteractionData);
            }
        }
    }

    private void Input_OnInteractionCanceled()
    {
        if (_currentInteractionData != null)
            _currentInteractionData.isHoldingInteraction = false;
    }

    private bool TryExitInteraction()
    {
        if (_currentInteractionControlData != null && _currentInteractionControlData.preventExitOnRequest ||
            LeanTween.isTweening(gameObject))
            return false;

        ExitInteraction();
        return true;
    }

    private void ExitInteraction()
    {
        if (_currentInteractionData != null)
        {
            _currentInteractionData.isExitRequested = true;
            _currentInteractionData.isHoldingInteraction = false;
            _currentInteractionData.input = Vector2.zero;

            _currentInteractionData = null;
            InteractionExited?.Invoke();
        }

        if (_currentInteractionControlData != null)
            _currentInteractionControlData = null;
    }

    private bool PerformRaycast(out RaycastHit hitInfo) => 
        Physics.Raycast(_raycastOrigin.position, _raycastOrigin.forward, out hitInfo,
            _rayMaxDistance, _detectionMask, QueryTriggerInteraction.Collide);

    
    void IInteractionHandler.Handle(ItemPlace place) => _itemController.DeleteItem();
    void IInteractionHandler.Handle(Item item) => _itemController.SetItem(item);
    void IInteractionHandler.Handle(IControlInteractable controlObject)
    {
        var data = controlObject.GetControlData();

        _currentInteractionControlData = data;
        ControlObjectInteracted?.Invoke(data);
    }
}