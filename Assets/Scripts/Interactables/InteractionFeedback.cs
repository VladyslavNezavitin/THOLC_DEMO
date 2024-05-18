using System;
using Unity.VisualScripting;

[Serializable]
public class InteractionFeedback
{
    public readonly string name;
    public readonly string description;
    public readonly bool interactionValid;

    public InteractionFeedback(string name, string description, bool interactionValid)
    {
        this.name = name;
        this.description = description;
        this.interactionValid = interactionValid;
    }
}
