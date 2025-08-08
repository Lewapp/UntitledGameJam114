using UnityEngine;

/// <summary>
/// Represents an interactable object that can be picked up and released by the player
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PickUp : MonoBehaviour, IInteractable
{
    #region Private Variables
    private Rigidbody rb; // Reference to the Rigidbody component for physics interactions
    #endregion

    #region Unity Methods
    private void Start()
    {
        // Ensure the Rigidbody component is present and set to non-kinematic by default
        rb = GetComponent<Rigidbody>();
    }
    #endregion

    #region Methods
    /// <summary>
    /// Toggles the interaction state of the object, either attaching it to or detaching it from the specified parent.
    /// </summary>
    public void Interact(InteractableData data)
    {
        // Check if the interactor is null or if the parent is not set
        if (data.parent == null)
        {
            transform.SetParent(null);
            rb.isKinematic = false; // Enable physics when released
            SetTeleportableState(true);
            return;
        }

        rb.isKinematic = true; // Disable physics when picked up
        transform.SetParent(data.parent); // Set the parent to the specified hold point
        transform.localPosition = Vector3.zero; // Reset position relative to the parent
        transform.localEulerAngles = Vector3.zero; // Reset rotation relative to the parent
        SetTeleportableState(false);
    }

    private void SetTeleportableState(bool enable)
    {
        ITeleportable teleportable = GetComponent<ITeleportable>();
        if (teleportable == null)
            return; // If the object does not implement ITeleportable, exit;

        teleportable.canTeleport = enable; // Set the teleportable state
    }
    #endregion
}
