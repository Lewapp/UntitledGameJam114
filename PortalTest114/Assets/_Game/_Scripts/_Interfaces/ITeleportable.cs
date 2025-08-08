using UnityEngine;

/// <summary>
/// Interface for objects that can be teleported through portals.
/// </summary>
public interface ITeleportable
{
    public void Teleport(Vector3 position, Quaternion rotation);
}
