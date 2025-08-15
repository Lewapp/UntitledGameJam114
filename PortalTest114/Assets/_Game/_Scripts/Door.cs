using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents a door that can be interacted with to open or close it, based on the number of interactors.
/// </summary>
/// <remarks>The door requires a specified number of interactors to open, as defined by the <see
/// cref="interactorsRequired"/> field. If <see cref="staysOpen"/> is set to true, the door will
/// remain open after interaction.</remarks>
public class Door : MonoBehaviour, IInteractable
{
    #region Inspector Variables
    [Header("Door Settings")]
    [SerializeField] private GameObject doorObject; // The GameObject representing the door that will be opened or closed
    [SerializeField] private bool staysOpen = false; // If true, the door will remain open after interaction
    [SerializeField] private int interactorsRequired = 1; // Number of interactors required to open the door
    [SerializeField] private TextMeshProUGUI[] interactorsLeftTXT; // UI elements to display the number of interactors required
    [SerializeField] private Light[] pointLights; // Array of PointLight components to control lighting effects on the door
    [SerializeField] private Color lockedColour = Color.red; // Color to indicate the door is locked
    [SerializeField] private Color unlockedColour = Color.green; // Color to indicate the door is unlocked
    #endregion

    #region Private Variables
    private bool isOpen = false; // Indicates whether the door is currently open or closed
    public List<GameObject> interactors; // List of GameObjects that are currently interacting with the door
    #endregion

    #region Unity Events
    private void Start()
    {
        // Ensure the doorObject is set and active at the start
        interactors = new List<GameObject>();
        UpdateLockStateVisuals(); // Initialise the lock state visuals
    }
    #endregion

    #region Methods
    /// <summary>
    /// Determines whether the specified GameObject is present in the interactor list.
    /// </summary>
    private bool CheckIfInInteractorList(GameObject subject)
    {
        GameObject foundSubject = null;

        // Iterate through the list of interactors to find the specified subject
        foreach (GameObject interactor in interactors)
        {
            // Check if the interactor matches the subject
            if (interactor != subject)
                continue;

            // If a match is found, set the foundSubject and break the loop
            foundSubject = interactor;
            break;
        }

        return (foundSubject != null);
    }

    private void UpdateLockStateVisuals()
    {
        foreach (TextMeshProUGUI requiredVisual in interactorsLeftTXT)
        {
            if (!requiredVisual)
                continue;

            // Update the text to show the number of interactors required
            requiredVisual.text = $"{interactorsRequired - interactors.Count}";
        }

        // Determine the current color based on whether the door is open or closed
        Color currentColour = (isOpen) ? unlockedColour : lockedColour;

        foreach (Light pointLight in pointLights)
        {
            if (!pointLight)
                continue;

            // Set the color of the point lights to indicate the door is unlocked
            pointLight.color = currentColour;
        }
    }
    #endregion

    #region Interfaces
    /// <summary>
    /// Toggles the interaction state of the specified interactor and updates the door's state accordingly.
    /// </summary>
    public void Interact(InteractableData data)
    {
        // Checks to see if the interactor is found in the list of interactors
        if (CheckIfInInteractorList(data.parent.gameObject))
        {
            interactors.Remove(data.parent.gameObject); // Remove the interactor if it is already in the list
        }
        else
        {
            interactors.Add(data.parent.gameObject); // Add the interactor to the list
        }

        isOpen = false; // Reset the door state to closed
        // Check if the number of interactors meets or exceeds the required count to open the door
        if (interactors.Count >= interactorsRequired)
        {
            isOpen = true;
        }

        UpdateLockStateVisuals(); // Update the visuals to reflect the current lock state

        // If the door is set to stay open, it will not toggle back to closed
        if (!isOpen && staysOpen)
            return;

        // Toggle the door's active state based on whether it is open or closed
        doorObject.SetActive(!isOpen);
    }
    #endregion

}
