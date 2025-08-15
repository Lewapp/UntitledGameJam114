using UnityEngine;

public interface IInteractable
{
    public void Interact(InteractableData data); // Method to be called when the object is interacted with
    public void DeInteract(InteractableData data) { } // Optional method to handle de-interaction
}

public struct InteractableData
{
    public GameObject interactor;
    public Transform parent;
    public bool isPressed; // Optional field to indicate if the interaction is a press action
}
