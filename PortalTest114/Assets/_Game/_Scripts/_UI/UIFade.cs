using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    public RawImage fadeImage;
    public float fadeDuration = 3f;
    public float fadeDelay = 1f; // Delay before starting the fade

    private bool isFading = false;
    private float currentFadeTime = 0f;
    private float currentDelayTime = 0f;
    private float startAlpha = 0f; // Starting alpha value for the fade
    private float desiredAlpha = 1f; // Desired alpha value for the fade

    private void Update()
    {
        if (!isFading)
            return;

        if (currentDelayTime < fadeDelay)
        {
            currentDelayTime += Time.deltaTime;
            return; // Wait for the delay before starting the fade
        }

        currentFadeTime += Time.deltaTime; ;
        if (currentFadeTime >= fadeDuration)
        {
            isFading = false;
            fadeImage.color = new Color(0f, 0f, 0f, desiredAlpha);
            return;
        }

        fadeImage.color = new Color(0f, 0f, 0f, Mathf.Lerp(startAlpha, desiredAlpha, currentFadeTime / fadeDuration));
    }

    public void StartFade()
    {
        isFading = true;
        startAlpha = 0f;
        desiredAlpha = 1f; // Set the desired alpha to 1 for fade in
        currentFadeTime = 0f;

        fadeImage.color = new Color(0f, 0f, 0f, startAlpha);
    }

    public void ReverseFade()
    {
        isFading = true;
        startAlpha = 1f; // Starting alpha value for reverse fade
        desiredAlpha = 0f; // Set the desired alpha to 0 for reverse fade
        currentFadeTime = 0f;

        fadeImage.color = new Color(0f, 0f, 0f, startAlpha);

    }
}
