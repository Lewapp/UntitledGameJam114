using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads a specified level when the player enters the trigger collider.
/// </summary>
public class LoadLevelTrigger : MonoBehaviour
{
    public string levelName; // The name of the level to load when the trigger is activated
    public LayerMask playerLayer; // The layer that the player must be on to trigger the level load

    private void OnTriggerEnter(Collider other)
    {
        if (string.IsNullOrEmpty(levelName))
        {
            Debug.LogWarning("Level name is not set in LoadLevelTrigger.");
            return; // Exit if the level name is not set
        }

        // Check if the other collider's layer matches the playerLayer mask
        if ((playerLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            SceneManager.LoadScene(levelName); // Load the specified level when an object enters the trigger collider
        }
    }
}
