using UnityEngine;

/// <summary>
/// Interface for objects that can be teleported through portals.
/// </summary>
public interface ITeleportable
{
    public bool canTeleport { get; set; } // Property to check if the object can be teleported

    public void Teleport(Vector3 position, Quaternion rotation);
}
