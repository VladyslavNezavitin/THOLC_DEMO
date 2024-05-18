using UnityEngine;

public sealed class Turnstile : Interactable
{
    protected override bool ValidateInteractionInternal(Item item) => item is Keycard;

    private void Start()
    {
        
    }
}
