using UnityEngine;

/// <summary>
/// Initialises the player's UI components at the start of the game.
/// </summary>
public class InitPlayerUI : MonoBehaviour
{
    #region Public Variables
    public UIFade fader; // Reference to the UIFade component for fade effects
    #endregion

    #region Unity Events
    void Start()
    {
        // Check if the fader is assigned and start the fade effect
        if (fader)
        {
            fader.ReverseFade();
        }
    }
    #endregion

}
