using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads a specified level when the player enters the trigger collider.
/// </summary>
public class LoadLevelTrigger : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private string levelName; // The name of the level to load when the trigger is activated
    [SerializeField] private LayerMask playerLayer; // The layer that the player must be on to trigger the level load
    [SerializeField] private UIFade fader; // Optional fade effect to apply before loading the level
    [SerializeField] private bool destroyNonPlayers; // If true, destroy non-player objects that enter the trigger collider
    #endregion

    #region Private Variables
    private float currentDelayTime = 0f; // Timer for the next scene delay
    private bool loadNextScene = false; // Flag to indicate if the next scene should be loaded
    #endregion

    #region Unity Events
    private void Start()
    {
        StaticSFX.instance?.PlayLevelStartSound(); // Play the level start sound 
    }

    private void Update()
    {
        if (!loadNextScene)
            return; // Exit if the next scene is not set to load

        if (currentDelayTime <=  0)
        {
            loadNextScene = false; // Reset the flag to prevent multiple loads
            SceneManager.LoadScene(levelName); // Load the specified level when an object enters the trigger collider
            return;
        }

        currentDelayTime -= Time.deltaTime; // Decrease the delay timer
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            if (destroyNonPlayers)
                Destroy(other.gameObject); // Destroy the object if it is not tagged as "Player" and destroyNonPlayers is true
            return; // Exit if the collider is not tagged as "Player"
        }

        if (string.IsNullOrEmpty(levelName))
        {
            Debug.LogWarning("Level name is not set in LoadLevelTrigger.");
            return; // Exit if the level name is not set
        }

        // If there is a player instance, stop its movement
        if (PlayerMovement.instance)
            PlayerMovement.instance.moveSpeed = 0f;

        // If respawning play respawn sound
        if (SceneManager.GetActiveScene().name == levelName)
        {
            StaticSFX.instance?.PlayRespawnSound(); // Play respawn sound if the level is the same as the current scene
        }
        else
        {
            StaticSFX.instance?.PlayNextLevelSound(); // Play next level sound if the level is different
        }

        // Check if the other collider's layer matches the playerLayer mask
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            loadNextScene = true; // Set the flag to load the next scene
        }

        if (!fader)
            return;

        fader.StartFade(); // Start the fade effect if a UIFade component is assigned
        currentDelayTime = fader.fadeDuration + fader.fadeDelay; // Set the delay time based on the fade duration and delay
    }
    #endregion
}
