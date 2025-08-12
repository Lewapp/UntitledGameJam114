using UnityEngine;
using System.Collections;
using static UnityEngine.EventSystems.EventTrigger;



#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles teleportation of the player and objects through portals.
/// This script should be attached to the portal GameObject.
/// </summary>
public class PortalTeleport : MonoBehaviour
{
    #region Public Variables
    [Header("Portal Settings")]
    public PortalTeleport linkedPortal; // The portal this one is linked to
    public LayerMask allowedEntryMask; // Layer mask to determine which objects can enter the portal
    public Vector3 teleportLocation; // The position offset for teleportation
    public Quaternion teleportRotation = Quaternion.identity; // The rotation offset for teleportation
    public bool forceSolo = false; // If true, objects cannot bring other objects with it through the portal
    #endregion

    #region Private Variables
    private GameObject expectingObject; // The object that is expected to enter the portal next, if any
    #endregion

    #region Unity Events
    private void OnTriggerEnter(Collider other)
    {
        if (!linkedPortal)
            return;

        if (expectingObject)
        {
            expectingObject = null; // Reset expecting object if one is already set
            return;
        }    

        // Check if the object is allowed to enter the portal based on its layer
        if ((allowedEntryMask.value & (1 << other.gameObject.layer)) == 0)
            return;

        // Teleport to the linked portal's position plus its teleportLocation offset
        other.GetComponent<ITeleportable>()?.Teleport(linkedPortal.transform.position + linkedPortal.teleportLocation, teleportRotation, linkedPortal.teleportRotation, forceSolo);
    
        linkedPortal.SetExpectingObject(other.gameObject); // Set the linked portal to expect this object next
    }
    #endregion

    #region Methods
    public void SetExpectingObject(GameObject newObject)
    {
        expectingObject = newObject; // Set the object that is expected to enter the portal next
    }
    #endregion

    #region Editor Code

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw a yellow wiresphere at the local teleport location
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + teleportLocation, 0.2f);

        // Draw a yellow arrow indicating the teleport direction
        Handles.color = Color.yellow; 
        Handles.ArrowHandleCap(0, transform.position + teleportLocation, teleportRotation, 0.5f, EventType.Repaint);
    }
#endif
}

#if UNITY_EDITOR
/// <summary>
/// Custom editor for PortalTeleport to allow manipulation of teleport location in the Scene view.
/// </summary>
[CustomEditor(typeof(PortalTeleport))]
public class TeleportHandle : Editor
{
    public void OnSceneGUI()
    {
        var portal = (PortalTeleport)target; // Get the current PortalTeleport instance being edited

        EditorGUI.BeginChangeCheck(); // Start checking for changes to the handle's position

        Vector3 currentPos = portal.transform.position + portal.teleportLocation;
        Quaternion currentRot = portal.teleportRotation;

        // Position Handle for teleport location
        Vector3 newPos = Handles.PositionHandle(currentPos, Quaternion.identity);
        // Rotation Handle for teleport rotation
        Quaternion newRot = Handles.RotationHandle(currentRot, currentPos);

        // If the position changed, update teleportLocation accordingly
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(portal, "Change Teleport Location"); // Record undo state for editor
            portal.teleportLocation = newPos - portal.transform.position; // Update teleportLocation offset relative to portal
            portal.teleportRotation = newRot; // Update teleport rotation
            EditorUtility.SetDirty(portal); // Mark the portal as dirty to ensure changes are saved
        }

        if (portal.linkedPortal)
        {
            // Set colour for connected objects to green
            Handles.color = Color.green;
            // Draw a line from the pressure plate to each connected object
            Handles.DrawDottedLine(portal.transform.position, portal.linkedPortal.transform.position, 5f);
        }
    }
}
#endif
#endregion


