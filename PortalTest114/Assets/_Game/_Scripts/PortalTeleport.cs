using UnityEngine;
using System.Collections;


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
    public Vector3 teleportLocation; // The position offset for teleportation
    public Quaternion teleportRotation = Quaternion.identity; // The rotation offset for teleportation
    public float teleportCooldown = 3f; // Cooldown time between teleports
    #endregion

    #region Private Variables
    private float currentCooldown; // Tracks the current cooldown time
    #endregion

    #region Unity Events
    private void Start()
    {
        currentCooldown = teleportCooldown; // Initialise cooldown to allow immediate teleportation
    }

    private void OnTriggerStay(Collider other)
    {
        // If cooldown is still active, do not teleport
        if (currentCooldown < teleportCooldown)
            return;

        // Teleport to the linked portal's position plus its teleportLocation offset
        other.GetComponent<ITeleportable>()?.Teleport(linkedPortal.transform.position + linkedPortal.teleportLocation, teleportRotation);

        // Start cooldowns on both this portal and the linked portal to prevent immediate back-and-forth teleporting
        StartCoroutine(linkedPortal.CoolDown());
        StartCoroutine(CoolDown());
    }
    #endregion

    #region Methods
    /// <summary>
    /// Implements a cooldown timer that waits for a specified duration before completing.
    /// </summary>
    public IEnumerator CoolDown()
    {
        currentCooldown = 0f; // Reset cooldown timer
        while (currentCooldown < teleportCooldown)
        {
            currentCooldown += Time.deltaTime; // Increment cooldown by the time passed since last frame
            yield return null; // Wait for the next frame
        }
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
    }
}
#endif
#endregion


