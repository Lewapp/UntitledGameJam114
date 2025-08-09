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
    public Material heldMaterial; // Material to apply to the held object 
    #endregion

    #region Private Variables
    private Camera playerCamera; // Reference to the player's camera
    private LayerMask layerMask; // Layer mask to filter out objects that can't be interacted with
    private Selected selectedObject; // Reference to the currently selected object
    private GameObject heldObject; // Reference to the object currently being held (if any)
    #endregion

    #region Unity Events
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
    /// <summary>
    /// Checks for selectable objects in front of the player and updates the selected object state.
    /// </summary>
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

        if (selectedObject)
        {
            // Call the 'DeSelect' method to remove its selected state
            selectedObject.DeSelect();
            selectedObject = null;
        }

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
    }

    /// <summary>
    /// Applies the currently held material to the specified game object to help see through the object.
    /// </summary>
    private void ApplyMaterial(GameObject objectToApply)
    {
        Renderer _selectedRenderer = objectToApply?.GetComponent<Renderer>();

        // If the object does not have a Renderer component, or if the held material is not set, do nothing
        if (!_selectedRenderer)
            return;

        _selectedRenderer.material = heldMaterial; // Apply the held material to the selected object
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
            IInteractable _interactable = selectedObject.GetComponent<IInteractable>();
            selectedObject.DeSelect(); // Deselect the object after interaction
            if (_interactable != null)
                _interactable.Interact(new InteractableData{ interactor = gameObject }); // Pass the player GameObject as the interactor

            // If the selected object is interactable, call its PickUp method
            IPickUpable _pickUpable = selectedObject.GetComponent<IPickUpable>();
            if (_pickUpable == null)
                return;

            _pickUpable.PickUp(holdPoint);

            heldObject = selectedObject.gameObject; // Store the currently held object
            ApplyMaterial(heldObject);
            return;
        }

        heldObject.GetComponent<IPickUpable>()?.PickUp(null);

        Selected _selected = heldObject.GetComponent<Selected>();
        if (_selected)
        {
            _selected.DeSelect(); // Deselect the held object
        }

        heldObject = null; // Clear the reference to the held object
    }

    #endregion
}
