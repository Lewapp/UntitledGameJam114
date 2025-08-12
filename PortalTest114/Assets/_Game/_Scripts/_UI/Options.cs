using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Options : MonoBehaviour
{
    [Header("Options Settings")]
    [SerializeField] private GameObject[] optionObjects;
    [SerializeField] private TextMeshProUGUI mouseSensTXT; // Text component to display mouse sensitivity
    [SerializeField] private TextMeshProUGUI gamepadSensTXT; // Text component to display gamepad sensitivity

    private void Start()
    {
        if (mouseSensTXT)
        {
            mouseSensTXT.text = PlayerPrefs.GetFloat("MouseSens").ToString("F2"); // Load mouse sensitivity from PlayerPrefs
        }

        if (gamepadSensTXT)
        {
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens").ToString("F2"); // Load gamepad sensitivity from PlayerPrefs
        }
    }

    public void DisplayOptions()
    {
        foreach (GameObject option in optionObjects)
        {
            if (!option)
                continue;

            option.SetActive(!option.activeSelf); 
        }
    }

    public void UpdateMouseSensitivity(Slider mouseSlider)
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSlider.value * 0.01f); // Save mouse sensitivity to PlayerPrefs
        if (mouseSensTXT)
        {
            mouseSensTXT.text = (PlayerPrefs.GetFloat("MouseSens") * 100f).ToString("F0"); // Update the text to display the new sensitivity value
        }
    }

    public void UpdateGamepadSensitivity(Slider gamepadSlider)
    {
        PlayerPrefs.SetFloat("GamepadSens", Mathf.Round(gamepadSlider.value)); // Save gamepad sensitivity to PlayerPrefs
        if (gamepadSensTXT)
        {
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens").ToString("F0"); // Update the text to display the new sensitivity value
        }
    }
}
