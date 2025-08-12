using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides functionality to perform fade-in and fade-out transitions on a UI element.
/// </summary>
public class UIFade : MonoBehaviour
{
    #region Public Variables
    public RawImage fadeImage; // The UI element to apply the fade effect to
    public float fadeDuration = 3f; // Duration of the fade effect in seconds
    public float fadeDelay = 1f; // Delay before starting the fade
    #endregion

    #region Private Variables
    private bool isFading = false; // Flag to indicate if a fade is currently in progress
    private float currentFadeTime = 0f; // Timer for the current fade effect
    private float currentDelayTime = 0f; // Timer for the delay before starting the fade
    private float startAlpha = 0f; // Starting alpha value for the fade
    private float desiredAlpha = 1f; // Desired alpha value for the fade
    #endregion

    #region Unity Events
    private void Update()
    {
        // Check if a fade is in progress
        if (!isFading)
            return;

        // If the fade is in progress, update the fade effect
        if (currentDelayTime < fadeDelay)
        {
            currentDelayTime += Time.deltaTime;
            return; // Wait for the delay before starting the fade
        }

       // Update the fade time 
        currentFadeTime += Time.deltaTime;

        // Check if the fade duration has been reached
        if (currentFadeTime >= fadeDuration)
        {
            isFading = false;
            fadeImage.color = new Color(0f, 0f, 0f, desiredAlpha);
            return;
        }

        // Calculate the current alpha value based on the fade time
        fadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(startAlpha, desiredAlpha, currentFadeTime / fadeDuration));
    }
    #endregion

    #region Methods
    /// <summary>
    /// Initiates a fade-in effect by transitioning the alpha value of the fade image from 0 to 1.
    /// </summary>
    public void StartFade()
    {
        isFading = true;
        startAlpha = 0f;  // Starting alpha value for normal fade
        desiredAlpha = 1f; // Set the desired alpha to 1 for fade in
        currentFadeTime = 0f;

        fadeImage.color = new Color(0f, 0f, 0f, startAlpha);
    }

    /// <summary>
    /// Initiates a reverse fade effect, transitioning the alpha value of the fade image  from fully opaque to fully
    /// transparent.
    /// </summary>
    public void ReverseFade()
    {
        isFading = true;
        startAlpha = 1f; // Starting alpha value for reverse fade
        desiredAlpha = 0f; // Set the desired alpha to 0 for reverse fade
        currentFadeTime = 0f;

        fadeImage.color = new Color(0f, 0f, 0f, startAlpha);

    }
    #endregion
}
