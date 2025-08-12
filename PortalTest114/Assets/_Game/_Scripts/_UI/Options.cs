using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides functionality for managing and displaying game options.
/// </summary>
public class Options : MonoBehaviour
{
    #region Inspector Variables
    [Header("Options Settings")]
    [SerializeField] private GameObject[] optionObjects; // Array of GameObjects representing the options menu items
    [SerializeField] private TextMeshProUGUI mouseSensTXT; // Text component to display mouse sensitivity
    [SerializeField] private TextMeshProUGUI gamepadSensTXT; // Text component to display gamepad sensitivity
    [SerializeField] private Slider mouseSensSlider; // Slider for mouse sensitivity
    [SerializeField] private Slider gamepadSensSlider; // Slider for gamepad sensitivity
    #endregion


    #region Unity Events
    private void Start()
    {
        if (mouseSensTXT)
            mouseSensTXT.text = PlayerPrefs.GetFloat("MouseSens").ToString("F0"); // Load mouse sensitivity from PlayerPrefs

        if (gamepadSensTXT)
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens").ToString("F0"); // Load gamepad sensitivity from PlayerPrefs

        if (mouseSensSlider)
            mouseSensSlider.value = PlayerPrefs.GetFloat("MouseSens") * 100f; // Set mouse sensitivity slider value

        if (gamepadSensSlider)
            gamepadSensSlider.value = PlayerPrefs.GetFloat("GamepadSens"); // Set gamepad sensitivity slider value
    }
    #endregion

    #region Methods
    /// <summary>
    /// Toggles the active state of each option in the collection.
    /// </summary>
    public void DisplayOptions()
    {
        foreach (GameObject option in optionObjects)
        {
            if (!option)
                continue;

            option.SetActive(!option.activeSelf); 
        }
    }

    /// <summary>
    /// Updates the mouse sensitivity setting based on the provided slider value.
    /// </summary>
    public void UpdateMouseSensitivity(Slider mouseSlider)
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSlider.value * 0.01f); // Save mouse sensitivity to PlayerPrefs
        if (mouseSensTXT)
        {
            mouseSensTXT.text = (PlayerPrefs.GetFloat("MouseSens") * 100f).ToString("F0"); // Update the text to display the new sensitivity value
        }
    }

    /// <summary>
    /// Updates the gamepad sensitivity setting based on the provided slider value.
    /// </summary>
    public void UpdateGamepadSensitivity(Slider gamepadSlider)
    {
        PlayerPrefs.SetFloat("GamepadSens", Mathf.Round(gamepadSlider.value)); // Save gamepad sensitivity to PlayerPrefs
        if (gamepadSensTXT)
        {
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens").ToString("F0"); // Update the text to display the new sensitivity value
        }
    }
    #endregion
}
