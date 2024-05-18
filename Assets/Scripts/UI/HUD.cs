using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image _centerMarker;
    [SerializeField] private TextMeshProUGUI _objectNameText;
    [SerializeField] private TextMeshProUGUI _objectDescriptionText;
    [SerializeField] private Sprite _alternativeMarkerSprite;
    [SerializeField] private Sprite _defaultMarkerSprite;

    private PlayerInteractions Interactions => PlayerContext.Instance.Interactions;

    private void OnEnable()
    {
        Interactions.InteractableDetected += Player_OnInteractableDetectedChanged;
    }

    private void OnDisable()
    {
        Interactions.InteractableDetected -= Player_OnInteractableDetectedChanged;
    }

    private void Player_OnInteractableDetectedChanged(InteractionFeedback feedback)
    {
        if (feedback == null)
        {
            _objectNameText.text = string.Empty;
            _objectDescriptionText.text = string.Empty;
            _centerMarker.sprite = _defaultMarkerSprite;
            return;
        }

        _objectNameText.text = feedback.name;
        _objectDescriptionText.text = feedback.description;

        _centerMarker.sprite = feedback.interactionValid ? _alternativeMarkerSprite : _defaultMarkerSprite;
    }
}
