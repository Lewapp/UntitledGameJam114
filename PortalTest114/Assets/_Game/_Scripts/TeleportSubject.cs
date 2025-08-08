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

    #region Unity Methods
    private void Start()
    {
        canTeleport = true; // Allow teleportation by default
    }
    #endregion

    #region Methods
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        if (!canTeleport) 
            return; // If teleportation is not allowed, exit the method
        transform.position = position; // Set the player's position to the teleport location
        transform.rotation = rotation; // Set the player's rotation to the teleport rotation
    }
    #endregion
}
