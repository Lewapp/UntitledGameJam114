using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides functionality for managing pause menu actions in a game, including pausing, resuming, navigating to the
/// main menu, and quitting the application.
public class PauseMenuButtons : MonoBehaviour
{
    #region Inspector Variables
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject[] pauseObjects; // Array of GameObjects representing the pause menu options
    [SerializeField] private GameObject pauseFirstSelect; // Reference to the first selectable object in the pause menu
    [SerializeField] private GameObject optionsFirstSelect; // Reference to the first selectable object in the options menu
    #endregion

    #region Private Variables
    private EventSystem eventSystem; // Reference to the EventSystem for UI navigation
    #endregion

    #region Unity Events
    private void Awake()
    {
        eventSystem = EventSystem.current; // Get the current EventSystem instance
    }
    #endregion

    #region Methods
    /// <summary>
    /// Pauses the game by activating pause menu objects, freezing game time, and enabling the cursor.
    /// </summary>
    public void PauseGame()
    {
        // Goes through each GameObject in the pauseObjects array and activates them
        foreach (GameObject pauseObject in pauseObjects)
        {
            if (!pauseObject)
                continue;

            pauseObject.SetActive(true); // Activate all pause menu objects
        }

        Time.timeScale = 0f; // Pause the game by setting time scale to zero
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        SetPauseControllerSelect(); // Set the first selectable object in the pause menu as selected
    }

    /// <summary>
    /// Resumes the game by deactivating pause menu objects, restoring the time scale, and locking the cursor.
    /// </summary>
    public void ResumeGame()
    {
        // Goes through each GameObject in the pauseObjects array and deactivates them
        foreach (GameObject pauseObject in pauseObjects)
        {
            if (!pauseObject)
                continue;

            pauseObject.SetActive(false); // Deactivate all pause menu objects
        }

        Time.timeScale = 1f; // Resume the game by setting time scale back to one
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Sets the currently selected GameObject in the EventSystem to the first selectable object in the options menu.
    /// </summary>
    public void SetOptionControllerSelect()
    {
        eventSystem.SetSelectedGameObject(null); // Clear the currently selected GameObject in the EventSystem
        eventSystem.SetSelectedGameObject(optionsFirstSelect); // Set the first selectable object in the options menu as selected
    }

    /// <summary>
    /// Sets the currently selected GameObject in the EventSystem to the first selectable object in the pause menu.
    /// </summary>
    public void SetPauseControllerSelect()
    {
        eventSystem.SetSelectedGameObject(null); // Clear the currently selected GameObject in the EventSystem
        eventSystem.SetSelectedGameObject(pauseFirstSelect); // Set the first selectable object in the pause menu as selected
    }

    /// <summary>
    /// Loads the main menu scene and resets the time scale to its default value.
    /// </summary>
    public void MainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset before loading a new scene
        // Load the main menu scene
        SceneManager.LoadScene("Scene_MainMenu");
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    public void QuitApplication()
    {
        // Quit the application
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    #endregion
}
