using UnityEngine;

/// <summary>
/// Reppresents whether this object is selected or not.
/// Should be used on objects that can be selected in the game.
/// </summary>
public class Selected : MonoBehaviour
{
    #region Public Variables
    [Header("Selection Settings")]
    public Material selectedMaterial; // Material to apply when selected
    #endregion

    #region Private Variables
    private Renderer objectRenderer; // Renderer component of the object
    private Material defaultMaterial; // Default material to revert to when not selected
    #endregion

    #region Unity Events
    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        defaultMaterial = objectRenderer?.material; // Store the default material
    }
    #endregion

    #region Methods
    /// <summary>
    /// Changes the material of the associated object to the selected material.
    /// </summary>
    public void Select()
    {
        // Check if the renderer and selected material are set
        if (!objectRenderer || !selectedMaterial)
            return;
           
        objectRenderer.material = selectedMaterial; // Change to selected material 
    }

    /// <summary>
    /// Reverts the object's material to its default state, effectively deselecting it.
    /// </summary>
    public void DeSelect()
    {
        // Check if the renderer and default material are set
        if (!objectRenderer || !defaultMaterial)
            return;

        objectRenderer.material = defaultMaterial; // Revert to default material
    }
    #endregion
}
