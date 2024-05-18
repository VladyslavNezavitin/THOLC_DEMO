using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private InteractionFeedbackData _feedbackData = new();

    public void Interact(InteractionData data) => InteractInternal(data);
    public bool ValidateInteraction(Item item) => ValidateInteractionInternal(item);
    public InteractionFeedback GetFeedback(Item item) => GetFeedbackInternal(item);

    protected virtual void InteractInternal(InteractionData data) { }
    protected virtual bool ValidateInteractionInternal(Item item) => true;
    protected virtual InteractionFeedback GetFeedbackInternal(Item item)
    {
        bool interactionValid = ValidateInteraction(item);
        string description = interactionValid ? _feedbackData.interactionAvailableDescription :
                                                _feedbackData.interactionUnavailableDescription;

        return new InteractionFeedback(_feedbackData.name, description, interactionValid);
    }
}

public interface IControlInteractable
{
    public InteractionControlData GetControlData();
}

public enum Axis
{
    X,
    Y,
    Z
}