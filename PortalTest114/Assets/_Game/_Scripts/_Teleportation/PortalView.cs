using UnityEngine;

public class PortalView : MonoBehaviour
{
    #region Public Variables
    public Camera portalCamera; // The camera that renders the portal view
    public Renderer targetPortalView; // The portal view that this camera will render to
    public Color portalColor = new Color(1f, 1f, 1f, 0.8f); // Default: white with 80% opacity
    #endregion

    #region Private Variables
    private RenderTexture portalRT; // The render texture for the portal camera
    private Material portalMat; // The material that uses the render texture
    #endregion

    #region Unity Events
    private void Start()
    {
        // Create a new RenderTexture for the portal camera
        portalRT = new RenderTexture(Screen.width / 2, Screen.height / 2, 24);
        portalCamera.targetTexture = portalRT;

        if (targetPortalView != null)
        {
            portalMat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            portalMat.SetFloat("_Surface", 1); 
            portalMat.SetFloat("_Blend", 0);   
            portalMat.SetFloat("_Cull", 2);   
            portalMat.SetColor("_BaseColor", portalColor); 
            portalMat.mainTexture = portalRT;

            // Set the portal texture
            portalMat.mainTexture = portalRT;

            // Apply the chosen color (includes alpha)
            portalMat.color = portalColor;

            // Assign it to the target portal renderer
            targetPortalView.material = portalMat;
        }
    }

    private void Update()
    {
        portalCamera.enabled = targetPortalView.isVisible;
        
    }
    #endregion
}