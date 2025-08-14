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
    public Vector3 teleportLocation; // The position offset for teleportation
    #endregion

    #region Inspector Variables
    [SerializeField] private LayerMask allowedEntryMask; // Layer mask to determine which objects can enter the portal
    [SerializeField] private bool forceSolo = false; // If true, objects cannot bring other objects with it through the portal

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for playing portal sounds
    [SerializeField] private Vector2 randomPitchRange = new Vector2(0.9f, 1.1f); // Range for random pitch variation when playing sounds
    #endregion

    #region Private Variables
    private GameObject expectingObject; // The object that is expected to enter the portal next, if any
    #endregion

    #region Unity Events
    private void Start()
    {
        if (audioSource)
            StaticSFX.instance?.InitialiseNewSource(audioSource); // Initialise the sfx audio if available

    }

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
        other.GetComponent<ITeleportable>()?.Teleport(linkedPortal.transform.position + linkedPortal.teleportLocation, transform.rotation, linkedPortal.transform.rotation, forceSolo);
        linkedPortal.SetExpectingObject(other.gameObject); // Set the linked portal to expect this object next

        PlayPortalSound(); // Play the portal sound on this portal
        linkedPortal.PlayPortalSound(); // Play the portal sound on the linked portal
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets the object that is expected to enter the portal next.
    /// </summary>
    public void SetExpectingObject(GameObject newObject)
    {
        expectingObject = newObject; // Set the object that is expected to enter the portal next
    }

    /// <summary>
    /// Randomises the pitch of the portal sound and plays it.
    /// </summary>
    public void PlayPortalSound()
    {
        if (!audioSource)
            return;

        audioSource.pitch = Random.Range(randomPitchRange.x, randomPitchRange.y); // Set a random pitch within the specified range
        audioSource.PlayOneShot(audioSource.clip); // Play the portal sound if available
    }
    #endregion

}

#region Editor Code
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

        if (portal.linkedPortal)
        {
            // Set colour for connected objects to green
            Handles.color = Color.green;
            // Draw a line from the pressure plate to each connected object
            Handles.DrawDottedLine(portal.transform.position, portal.linkedPortal.transform.position, 5f);
        }

        SceneView.RepaintAll();
    }
}
#endif
#endregion


