using System;
using UnityEngine;

[RequireComponent(typeof(KeypadBehaviour))]
public sealed class KeypadCardReader : Interactable, IControlInteractable
{
    public event Action CardInserted;

    [Header("Tweening")]
    [SerializeField] private Axis _cardMovementAxis;
    [SerializeField] private float _cardMovementDistance;
    [SerializeField] private Transform _cardStartPoint;

    private KeypadBehaviour _keypad;
    private InteractionData _interactionData;

    private void Awake() => _keypad = GetComponent<KeypadBehaviour>();    
    protected override bool ValidateInteractionInternal(Item item) => item is Keycard;

    protected override void InteractInternal(InteractionData data)
    {
        Keycard keycard = data.item as Keycard;
        if (keycard != null && !LeanTween.isTweening(keycard.gameObject))
        {
            _interactionData = data;
            data.handler.Handle(this);

            CardInserted?.Invoke();

            ProcessKeycardScan(keycard);
        }
    }

    public InteractionControlData GetControlData() => new InteractionControlData() 
    {
        preventExitOnRequest = true,
        freezeItem = true 
    };

    private void ProcessKeycardScan(Keycard keycard)
    {
        Transform parent = keycard.transform.parent;
        
        keycard.transform.parent = _cardStartPoint;
        keycard.transform.localPosition = Vector3.zero;
        keycard.transform.localRotation = Quaternion.identity;

        Vector3 targetPosition = keycard.transform.position + 
            _cardMovementAxis.ToTransformDirection(keycard.transform) * _cardMovementDistance;

        bool scanned = false;

        LeanTween.move(keycard.gameObject, targetPosition, 1f)
        .setEaseInOutQuint()
        .setOnUpdate((float progress) =>
        {
            if (!scanned && progress > 0.8f)
            {
                _keypad.Base.TryEnter(keycard.Key);
                scanned = true;
            }
        })
        .setOnComplete(() =>
        {
            keycard.transform.parent = parent;
            keycard.transform.localPosition = Vector3.zero;
            keycard.transform.localRotation = Quaternion.identity;

            _interactionData.forceExitCallback?.Invoke();
        });
    }
}
