using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the player's camera zoom functionality.
/// </summary>
public class PlayerZoom : MonoBehaviour
{
    #region Inspector Variables
    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 2f; // Speed of zooming in and out
    [SerializeField] private float zoomFOV = 40f; // Target zoom level
    #endregion

    #region Private Variables
    private Camera playerCamera; // Reference to the player's camera
    private float defaultFOV; // Default field of view

    private Coroutine activeZoom; // Reference to the active zoom coroutine, if any
    #endregion

    #region Unity Events
    public void Start()
    {
        playerCamera = Camera.main; // Get the main camera
        if (playerCamera != null)
        {
            defaultFOV = playerCamera.fieldOfView; // Store the default FOV
        }
        else
        {
            Debug.LogError("PlayerZoom: No main camera found.");
        }
    }
    #endregion

    #region Methiods
    private void ResetZoom()
    {
        if (activeZoom != null)
        {
            StopCoroutine(activeZoom);
            activeZoom = null; // Reset the active zoom coroutine
        }
    }
    #endregion

    #region Input Actions
    public void ZoomCamera(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ResetZoom();
            // Start zooming in
            activeZoom = StartCoroutine(ZoomIn());
            PlayerMovement.instance.sensitivityMultiplier = 0.5f; // Reduce sensitivity while zoomed in
        }
        else if (context.canceled)
        {
            ResetZoom();
            // Stop zooming in and return to default FOV
            activeZoom = StartCoroutine(ZoomOut());
            PlayerMovement.instance.sensitivityMultiplier = 1f; // Restore sensitivity after zoom
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator ZoomIn()
    {
        while (playerCamera.fieldOfView > zoomFOV)
        {
            playerCamera.fieldOfView -= zoomSpeed * Time.deltaTime; // Decrease FOV for zoom effect
            yield return null; // Wait for the next frame
        }

        playerCamera.fieldOfView = zoomFOV; // Ensure FOV does not go below target zoom level
    }

    private IEnumerator ZoomOut()
    {
        while (playerCamera.fieldOfView < defaultFOV)
        {
            playerCamera.fieldOfView += zoomSpeed * Time.deltaTime; // Increase FOV to return to default
            yield return null; // Wait for the next frame
        }

        playerCamera.fieldOfView = defaultFOV; // Ensure FOV returns to default value
    }
    #endregion
}
