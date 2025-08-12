using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides functionality for handling main menu button interactions.
/// </summary>

public class MainMenuButtons : MonoBehaviour
{
    #region Public Variables
    [Header("Main Menu Settings")]
    public GameObject startDoor;
    public UIFade fader;
    #endregion

    #region Public Variables
    private bool isStarting = false;
    private float startDelay = 3f; // Delay before starting the game
    #endregion

    #region Unity Events
    private void Start()
    {
        if (fader)
        {
            startDelay = fader.fadeDuration + fader.fadeDelay; // Use the fade duration from the UIFade component
        }
    }

    private void Update()
    {
        // Check if the level loading is in the starting state
        if (!isStarting)
            return;

        // Check if the game is starting and if the fade duration has passed
        if (startDelay > 0f)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        // If the delay has passed, load the first level
        if (startDelay <= 0f)
        {
            isStarting = false;
            SceneManager.LoadScene("Scene_Level_1");
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initiates the game start sequence, interacting with the starting door to drop the box, if available, and triggering the fade effect.
    public void StartGame()
    {
        // Check if the start door is assigned
        if (startDoor)
        {
            // Open the door, dropping the box
            IInteractable interactable = startDoor.GetComponent<IInteractable>();
            interactable?.Interact(new InteractableData { interactor = gameObject, parent = transform });
        }

        // Start the level loading sequence
        isStarting = true;
        fader.StartFade();
    }

    /// <summary>
    /// Closes the application when the quit button is pressed.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
