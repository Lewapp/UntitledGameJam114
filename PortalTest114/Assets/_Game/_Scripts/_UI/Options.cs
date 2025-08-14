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
    // Sensitivity Settings
    [SerializeField] private TextMeshProUGUI mouseSensTXT; // Text component to display mouse sensitivity
    [SerializeField] private TextMeshProUGUI gamepadSensTXT; // Text component to display gamepad sensitivity
    [SerializeField] private Slider mouseSensSlider; // Slider for mouse sensitivity
    [SerializeField] private Slider gamepadSensSlider; // Slider for gamepad sensitivity
    // Volume Settings
    [SerializeField] private TextMeshProUGUI musicVolumeTXT; // Text component to display music volume
    [SerializeField] private TextMeshProUGUI sfxVolumeTXT; // Text component to display sound effects volume
    [SerializeField] private Slider musicVolumeSlider; // Slider for music volume
    [SerializeField] private Slider sfxVolumeSlider; // Slider for sound effects volume
    #endregion


    #region Unity Events
    private void Start()
    {
        // Initialise the sensitivity settings from PlayerPrefs
        if (mouseSensTXT)
            mouseSensTXT.text = PlayerPrefs.GetFloat("MouseSens", 5f).ToString("F0"); // Load mouse sensitivity from PlayerPrefs
        if (gamepadSensTXT)
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens", 250f).ToString("F0"); // Load gamepad sensitivity from PlayerPrefs

        if (mouseSensSlider)
            mouseSensSlider.value = PlayerPrefs.GetFloat("MouseSens", 5f) * 100f; // Set mouse sensitivity slider value
        if (gamepadSensSlider)
            gamepadSensSlider.value = PlayerPrefs.GetFloat("GamepadSens", 250f); // Set gamepad sensitivity slider value

        // Initialise the volume settings from PlayerPrefs
        if (musicVolumeTXT)
            musicVolumeTXT.text = PlayerPrefs.GetFloat("MusicVolume", 1f).ToString("F0"); // Load music volume from PlayerPrefs
        if (sfxVolumeTXT)
            sfxVolumeTXT.text = PlayerPrefs.GetFloat("SFXVolume", 1f).ToString("F0"); // Load sound effects volume from PlayerPrefs

        if (musicVolumeSlider)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f) * 100f; // Set music volume slider value
        if (sfxVolumeSlider)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f) * 100f; // Set sound effects volume slider value
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
    public void UpdateMouseSensitivity()
    {
        PlayerPrefs.SetFloat("MouseSens", mouseSensSlider.value * 0.01f); // Save mouse sensitivity to PlayerPrefs
        if (mouseSensTXT)
        {
            mouseSensTXT.text = (PlayerPrefs.GetFloat("MouseSens") * 100f).ToString("F0"); // Update the text to display the new sensitivity value
        }
    }

    /// <summary>
    /// Updates the gamepad sensitivity setting based on the provided slider value.
    /// </summary>
    public void UpdateGamepadSensitivity()
    {
        PlayerPrefs.SetFloat("GamepadSens", Mathf.Round(gamepadSensSlider.value)); // Save gamepad sensitivity to PlayerPrefs
        if (gamepadSensTXT)
        {
            gamepadSensTXT.text = PlayerPrefs.GetFloat("GamepadSens").ToString("F0"); // Update the text to display the new sensitivity value
        }
    }

    /// <summary>
    /// Updates the music volume setting based on the provided slider value.
    /// </summary>
    public void UpdateMusicVolume()
    {
        PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value * 0.01f); // Save music volume to PlayerPrefs
        if (musicVolumeTXT)
        {
            musicVolumeTXT.text = (PlayerPrefs.GetFloat("MusicVolume") * 100f).ToString("F0"); // Update the text to display the new volume value
        }

        Ambiance.instance?.UpdateVolume(); // Update the volume in the Ambiance instance if it exists
    }

    /// <summary>
    /// Updates the sfx volume setting based on the provided slider value.
    /// </summary>
    public void UpdateSfxVolume()
    {
        PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value * 0.01f); // Save sound effects volume to PlayerPrefs
        if (sfxVolumeTXT)
        {
            sfxVolumeTXT.text = (PlayerPrefs.GetFloat("SFXVolume") * 100f).ToString("F0"); // Update the text to display the new volume value
        }

        StaticSFX.instance?.UpdateVolumes(); // Update the volumes in the StaticSFX instance if it exists
    }
    #endregion
}
