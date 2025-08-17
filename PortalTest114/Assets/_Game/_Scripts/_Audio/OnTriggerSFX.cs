using UnityEngine;

/// <summary>
/// A simple component that plays a sound effect when the GameObject's trigger collider is activated.
/// </summary>
public class OnTriggerSFX : MonoBehaviour
{
    #region Inspector Variables
    [SerializeField] private AudioClip soundEffect; // The sound effect to play when the trigger is activated
    [SerializeField] private AudioSource audioSource; // The AudioSource component to play the sound effect
    [SerializeField] private LayerMask triggerLayer; // Layer mask to specify which layers can trigger the sound effect
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f); // Range of pitch variation for the sound effect
    #endregion

    #region Unity Events
    void Start()
    {
        if (audioSource)
        {
            audioSource.clip = soundEffect; // Set the clip to the AudioSource

            if (StaticSFX.instance)
                StaticSFX.instance.InitialiseNewSource(audioSource); // Register this SFX with the StaticSFXManager
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (audioSource && soundEffect && ((1 << other.gameObject.layer) & triggerLayer) != 0)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y); // Randomise the pitch within the specified range
            audioSource.PlayOneShot(soundEffect); // Play the sound effect when the trigger is activated
        }
    }
    #endregion

}
