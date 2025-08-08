using UnityEngine;

public interface IInteractable
{
    public void Interact(InteractableData data); // Method to be called when the object is interacted with
}

public struct InteractableData
{
    public GameObject interactor;
    public Transform parent;
}
