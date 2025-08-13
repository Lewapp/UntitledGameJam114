using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Provides functionality for handling main menu button interactions.
/// </summary>
public class MainMenuButtons : MonoBehaviour
{
    #region Inspector Variables
    [Header("Main Menu Settings")]
    [SerializeField] private GameObject startDoor; // Reference to the door GameObject that will be interacted when starting game
    [SerializeField] private UIFade fader; // Reference to the UIFade component for fade effects
    [SerializeField] private TextMeshProUGUI displayedLevelSelection; // Text element to display the current level selection
    #endregion

    #region Private Variables
    private bool isStarting = false; // Flag to indicate if the game is in the starting state
    private float startDelay = 3f; // Delay before starting the game
    private string levelToLoad = "Scene_Level_1"; // Default level to load
    private int currentLevelIndex = 0; // Index of the current level in the selection
    #endregion

    #region Unity Events
    private void Start()
    {
        if (fader)
        {
            startDelay = fader.fadeDuration + fader.fadeDelay; // Use the fade duration from the UIFade component
        }

        Cursor.lockState = CursorLockMode.Confined; // Unlock the cursor for the main menu
        Cursor.visible = true; // Make the cursor visible
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
            SceneManager.LoadScene(levelToLoad);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initiates the game start sequence, interacting with the starting door to drop the box, if available, and triggering the fade effect.
    public void StartGame()
    {
        if (isStarting)
            return;

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

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// Advances the current level selection to the next level in the sequence.
    /// </summary>
    public void ChangeLevelSelect()
    {
        if (isStarting)
            return;

        currentLevelIndex++;
        switch (currentLevelIndex)
        {
            case 1:
                levelToLoad = "Scene_Level_2";
                break;
            case 2:
                levelToLoad = "Scene_Level_3";
                break;
            case 3:
                levelToLoad = "Scene_Level_4";
                break;
            default:
                currentLevelIndex = 0; // Reset to the first level if out of bounds
                levelToLoad = "Scene_Level_1";
                break;
        }

        if (displayedLevelSelection)
            displayedLevelSelection.text = $"Level: {currentLevelIndex + 1}";
    }
    #endregion
}
