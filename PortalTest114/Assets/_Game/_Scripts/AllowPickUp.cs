using UnityEngine;

/// <summary>
/// Represents an interactable object that can be picked up and released by the player
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class AllowPickUp : MonoBehaviour, IPickUpable
{
    #region Interface Variables
    public PickUpMode dropMode { get; set; }
    #endregion

    #region Inspector Variables
    [SerializeField] private AudioSource pickUpSound; // Sound to play when the object is picked up
    #endregion 

    #region Private Variables
    private Rigidbody rb; // Reference to the Rigidbody component for physics interactions
    private BoxCollider boxCollider; // Reference to the BoxCollider component for interaction detection
    #endregion

    #region Unity Events
    private void Start()
    {
        dropMode = PickUpMode.Drop; // Initialise the drop mode to drop

        // Ensure the Rigidbody component is present and set to non-kinematic by default
        rb = GetComponent<Rigidbody>();
        // Ensure the BoxCollider component is present
        boxCollider = GetComponent<BoxCollider>(); 

        if (pickUpSound)
            StaticSFX.instance?.InitialiseNewSource(pickUpSound); // Initialise the pick up sound if available
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the teleportable state of the object.
    /// </summary>
    private void SetTeleportableState(bool enable)
    {
        ITeleportable teleportable = GetComponent<ITeleportable>();
        if (teleportable == null)
            return; // If the object does not implement ITeleportable, exit;

        teleportable.canTeleport = enable; // Set the teleportable state
    }

    private void ApplyFowardForce()
    {
        if (dropMode != PickUpMode.Throw)
        {
            pickUpSound?.PlayOneShot(pickUpSound.clip); // Play the pick-up sound if available
            return; // If the drop mode is not Throw, do not apply force
        }


        // Apply a forward force to the object if it has a Rigidbody component
        if (rb != null)
        {
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse); // Adjust the force as needed
        }
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// Toggles the interaction state of the object, either attaching it to or detaching it from the specified parent.
    /// </summary>
    public void PickUp(Transform holdPoint)
    {
        // Check if the interactor is null or if the parent is not set
        if (holdPoint == null)
        {
            rb.isKinematic = false; // Enable physics when released
            boxCollider.enabled = true; // Re-enable the collider for further interactions

            transform.SetParent(null);
            SetTeleportableState(true);

            ApplyFowardForce(); // Apply a forward force when the object is released
            return;
        }

        pickUpSound?.PlayOneShot(pickUpSound.clip); // Play the pick-up sound if available

        rb.isKinematic = true; // Disable physics when picked up
        boxCollider.enabled = false; // Disable the collider to prevent further interactions

        transform.SetParent(holdPoint); // Set the parent to the specified hold point
        transform.localPosition = Vector3.zero; // Reset position relative to the parent
        transform.localEulerAngles = Vector3.zero; // Reset rotation relative to the parent
        SetTeleportableState(false);
    }
    #endregion
}
