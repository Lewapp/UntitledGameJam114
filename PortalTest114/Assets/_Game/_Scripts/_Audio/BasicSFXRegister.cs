using UnityEngine;

/// <summary>
/// A basic component that registers an AudioSource with the StaticSFXManager.
/// </summary>
public class BasicSFXRegister : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private AudioSource audioSource; // The AudioSource component to register
    #endregion

    #region Unity Events
    private void Start()
    {
        if (audioSource)
        {
            // Register this AudioSource with the StaticSFXManager
            if (StaticSFX.instance)
                StaticSFX.instance.InitialiseNewSource(audioSource);
        }
    }
    #endregion
}
