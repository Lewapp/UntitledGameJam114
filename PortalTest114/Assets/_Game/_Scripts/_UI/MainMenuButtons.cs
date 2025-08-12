using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject startDoor;
    public UIFade fader;

    private bool isStarting = false;
    private float startDelay = 3f; // Delay before starting the game

    private void Start()
    {
        if (fader)
        {
            startDelay = fader.fadeDuration + fader.fadeDelay; // Use the fade duration from the UIFade component
        }
    }

    private void Update()
    {
        if (!isStarting)
            return;

        // Check if the game is starting and if the fade duration has passed
        if (startDelay > 0f)
        {
            startDelay -= Time.deltaTime;
            return;
        }

        if (startDelay <= 0f)
        {
            isStarting = false;
            SceneManager.LoadScene("Scene_Level_1");
        }
    }
    public void StartGame()
    {
        if (startDoor)
        {
            IInteractable interactable = startDoor.GetComponent<IInteractable>();
            interactable?.Interact(new InteractableData { interactor = gameObject, parent = transform });
        }

        isStarting = true;
        fader.StartFade();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
