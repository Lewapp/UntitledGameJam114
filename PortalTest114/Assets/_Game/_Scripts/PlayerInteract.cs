using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the player's interactions in the game.
/// Must only be attached to the player GameObject.
/// </summary>
public class PlayerInteract : MonoBehaviour
{
    #region Public Variables
    [Header("Interaction Settings")]
    public float interactionDistance = 3f; // Distance within which the player can interact with objects
    public Transform holdPoint; // Point where the object will be held when interacted with
    #endregion

    #region Private Variables
    private Camera playerCamera; // Reference to the player's camera
    private LayerMask layerMask; // Layer mask to filter out objects that can't be interacted with
    private Selected selectedObject; // Reference to the currently selected object
    private GameObject heldObject; // Reference to the object currently being held (if any)
    #endregion

    #region Unity Methods
    private void Start()
    {
        playerCamera = Camera.main; // Get the main camera in the scene
        layerMask = LayerMask.GetMask("Selectable"); // Set the layer mask to Character layer
    }

    private void FixedUpdate()
    {
        CheckForSelectableObjects();
    }
    #endregion

    #region Methods
    private void CheckForSelectableObjects()
    {
        // If the player is already holding an object, skip checking for other selectable objects
        if (heldObject)
            return;

        // Calculate the forward direction based on the player's camera orientation
        Vector3 _direction = playerCamera.transform.TransformDirection(Vector3.forward);

        #if UNITY_EDITOR
        // In the editor, draw a red debug ray to visualize the interaction check
        Debug.DrawRay(transform.position, _direction * interactionDistance, Color.red, 0.1f);
        #endif

        // Perform a raycast forward from the player's position within the set interaction distance
        // Only detect objects on the specified layerMask
        if (Physics.Raycast(transform.position, _direction, out RaycastHit _hit, interactionDistance, layerMask))
        {
            // Try to get the 'Selected' component from the object we hit
            selectedObject = _hit.transform.GetComponent<Selected>();

            // If the hit object does not have a 'Selected' component, stop here
            if (!selectedObject)
                return;

            // Call the 'Select' method to visually mark the object as selected
            selectedObject.Select();
        }
        else if (selectedObject) // If nothing was hit, but we previously had a selected object
        {
            // Call the 'DeSelect' method to remove its selected state
            selectedObject.DeSelect();

            // Clear the stored reference since nothing is currently selected
            selectedObject = null;
        }
    }
    #endregion

    #region Input Actions
    public void Interact(InputAction.CallbackContext context)
    {
        // Check if the interaction input was performed and if there is a selected object
        if (!context.performed || !selectedObject)
            return;

        if (holdPoint == null)
        {
            Debug.LogError("Hold Point is not set on PlayerInteract script.");
        }

        if (!heldObject)
        {
            IInteractable interactable = selectedObject.GetComponent<IInteractable>();
            selectedObject.DeSelect(); // Deselect the object after interaction
            if (interactable == null)
                return;

            interactable.Interact(new InteractableData
            {
                interactor = gameObject, // Set the interactor to this player GameObject
                parent = holdPoint // Set the parent to the hold point
            });

            heldObject = selectedObject.gameObject; // Store the currently held object
            return;
        }

        heldObject.GetComponent<IInteractable>()?.Interact(new InteractableData
        {
            interactor = gameObject, // Set the interactor to this player GameObject
            parent = null // Clear the parent to release the object
        });

        heldObject = null; // Clear the reference to the held object
    }

    #endregion
}
