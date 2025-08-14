using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Represents a pressure plate that triggers interactions with connected objects when activated.
/// </summary>
public class PressurePlate : MonoBehaviour
{
    #region Inspector Variables
    [Header("Button Settings")]
    [SerializeField] private GameObject[] connectedObjects; // Objects that will be interacted with when the pressure plate is activated
    [SerializeField] private Vector3 triggerSize; // Size of the trigger area for the pressure plate
    [SerializeField] private LayerMask ignoreLayer; // Layer mask to ignore certain layers during interaction
    [SerializeField] private GameObject offVisual; // Visual representation of the pressure plate when not pressed
    [SerializeField] private GameObject onVisual; // Visual representation of the pressure plate when not pressed

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource; // Audio source for playing sounds
    [SerializeField] private Vector2 offOnPitch; // Pitch range for the sound when the pressure plate is pressed or released
    #endregion

    #region Private Variables
    private bool isPressed = false; // Indicates whether the pressure plate is currently pressed
    private List<Collider> hitColliders = new List<Collider>(); // The object that is currently pressing the pressure plate
    #endregion

    #region Unity Events
    private void Start()
    {
        if (audioSource && StaticSFX.instance)
            StaticSFX.instance.InitialiseNewSource(audioSource); // Initialise the sfx audio if available
    }

    private void FixedUpdate()
    {
        bool _successHit = false; // Reset pressed state at the start of each FixedUpdate
        hitColliders = Physics.OverlapBox(transform.position, triggerSize, Quaternion.identity).ToList();
        foreach (Collider _hit in hitColliders)
        {
            if ((ignoreLayer.value & (1 << _hit.transform.gameObject.layer)) != 0)
            {
                continue;
            }

            _successHit = true; // Set pressed state if any object is detected in the trigger area
            break;
        }

        if (_successHit != isPressed)
        {
            isPressed = _successHit; // Update pressed state based on whether an object is detected
            InteractWithObjects(); // Call interaction method to handle the change in state

            // Visuals
            offVisual?.SetActive(!isPressed); // Toggle off visual based on pressed state
            onVisual?.SetActive(isPressed); // Toggle on visual based on pressed state

            // Audio
            if (!audioSource)
                return;

            audioSource.pitch = (isPressed) ? offOnPitch.y : offOnPitch.x; // Set audio pitch based on pressed state
            audioSource.PlayOneShot(audioSource.clip); // Play the sound effect
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Iterates through connected objects and invokes their interaction behaviour if they implement the <see
    /// cref="IInteractable"/> interface.
    /// </summary>
    private void InteractWithObjects()
    {
        // Iterate through all connected objects and call their Interact method if they implement IInteractable
        foreach (GameObject obj in connectedObjects)
        {
            if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact(new InteractableData
                {
                    interactor = gameObject,
                    parent = transform
                });
            }
        }
    }
    #endregion

    #region Editor Code
#if UNITY_EDITOR
    [CustomEditor(typeof(PressurePlate))]
    [CanEditMultipleObjects]
    public class PressurePlateEditor : Editor
    {
        public void OnSceneGUI()
        {
            var pressurePlate = (PressurePlate)target;

            Vector3 centre = pressurePlate.transform.position;
            Vector3 size = pressurePlate.triggerSize;

            EditorGUI.BeginChangeCheck(); // Start checking for changes to the trigger size
            Vector3 newSize = Handles.ScaleHandle(size, centre, Quaternion.identity, HandleUtility.GetHandleSize(centre));

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(pressurePlate, "Change Pressure Plate Size"); // Record the change for undo
                pressurePlate.triggerSize = newSize; // Update the trigger size
            }

            Handles.color = Color.yellow; // Set handle color to yellow
            Handles.DrawWireCube(centre, newSize); // Draw a wireframe cube to visualize the trigger area

            foreach (GameObject obj in pressurePlate.connectedObjects)
            {
                if (obj == null) continue; // Skip null objects

                // Set colour for connected objects to green
                Handles.color = Color.green; 
                // Draw a line from the pressure plate to each connected object
                Handles.DrawDottedLine(pressurePlate.transform.position, obj.transform.position, 5f);
            }
        }
    }
#endif
    #endregion
}
