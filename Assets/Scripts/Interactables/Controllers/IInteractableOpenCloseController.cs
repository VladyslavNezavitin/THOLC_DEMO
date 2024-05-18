using System;

public interface IInteractableController
{
    bool CanInteract { get; }
    void Initialize(IOpenable openable, IClosable closable);
}
