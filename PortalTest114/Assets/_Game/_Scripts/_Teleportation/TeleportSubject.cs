using UnityEngine;

/// <summary>
/// Represents a subject that can be teleported to a specified position and rotation.
/// Will be used when object is interacting with portals.
/// Mustn't be attached to the player GameObject, only GameObjects that can be teleported.
/// </summary>
public class TeleportSubject : MonoBehaviour, ITeleportable
{
    #region Interface Variables
    public bool canTeleport { get ; set; }
    #endregion

    #region Public Variables
    public Rigidbody rb; // Reference to the Rigidbody component for physics interactions
    #endregion

    #region Unity Events
    private void Start()
    {
        canTeleport = true; // Allow teleportation by default
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component if it exists
    }
    #endregion

    #region Interfaces
    public void Teleport(Vector3 position, Quaternion entryRotation, Quaternion exitRotation, bool forceSolo)
    {
        if (!canTeleport)
            return;

        transform.position = position;
        if (rb)
        {
            // Get relative rotation between entry and exit portal
            Quaternion relativeRotation = exitRotation * Quaternion.Inverse(entryRotation);

            // Rotate velocity by this relative rotation
            Vector3 newVelocity = relativeRotation * rb.linearVelocity;
            rb.linearVelocity = newVelocity;
        }

        transform.rotation = exitRotation;
    }
    #endregion
}
