using UnityEngine;

public class LookAtMainCamera : MonoBehaviour
{
    private Transform mainCamera;

    private void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main?.transform;
        // If no main camera is found, log a warning
        if (mainCamera == null)
        {
            Debug.LogWarning("No main camera found. LookAtMainCamera will not function correctly.");
        }
    }

    private void LateUpdate()
    {
        if (mainCamera == null)
            return;

        transform.LookAt(mainCamera); // Make this GameObject look at the main camera
    }
}

