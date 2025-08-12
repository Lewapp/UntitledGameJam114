using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtons : MonoBehaviour
{
    [Header("Pause Menu Settings")]
    [SerializeField] private GameObject[] pauseObjects; // Array of GameObjects representing the pause menu options

    public void PauseGame()
    {


        foreach (GameObject pauseObject in pauseObjects)
        {
            if (!pauseObject)
                continue;

            pauseObject.SetActive(true); // Activate all pause menu objects
        }

        Time.timeScale = 0f; // Pause the game by setting time scale to zero
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
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

    public void MainMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset before loading a new scene
        // Load the main menu scene
        SceneManager.LoadScene("Scene_MainMenu");
    }

    public void QuitApplication()
    {
        // Quit the application
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
