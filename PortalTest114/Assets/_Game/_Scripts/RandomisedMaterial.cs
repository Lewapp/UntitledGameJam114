using UnityEngine;

/// <summary>
/// Randomly assigns a material from a predefined list to the Renderer component of the GameObject.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class RandomisedMaterial : MonoBehaviour
{
    #region Public Variables
    public Material[] materials; // Array of materials to choose from
    #endregion

    #region Private Variables
    private Renderer thisRenderer; // Renderer component of the GameObject
    #endregion

    #region Unity Events
    private void Awake()
    {
        thisRenderer = GetComponent<Renderer>(); // Get the Renderer component attached to this GameObject
        SelectMaterial();
    }
    #endregion

    #region Methods 
    /// <summary>
    /// Randomly selects a material from the available materials array and assigns it to the renderer.
    /// </summary>
    private void SelectMaterial()
    {
        // Check if the materials array is empty or null
        if (materials.Length <= 0)
            return;

        thisRenderer.material = materials[Random.Range(0, materials.Length)]; // Randomly select a material from the array and assign it to the renderer
    }
    #endregion
}
