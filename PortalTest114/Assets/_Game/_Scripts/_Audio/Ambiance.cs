using UnityEngine;

public class Ambiance : MonoBehaviour
{
    public static Ambiance instance { get; private set; }

    [SerializeField] private AudioSource ambianceAudioSource; // Reference to the AudioSource component for ambiance sounds

    private float initialVolume = 0.5f; // Default volume for ambiance sound

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (transform.parent)
        {
            transform.SetParent(null); // Detach from parent if it exists
        }

        instance = this;
        // Ensure the GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (ambianceAudioSource == null)
        {
            Debug.LogError("Ambiance AudioSource is missing from " + this.name);
        }
        else
        {
            initialVolume = ambianceAudioSource.volume; // Store the initial volume

            ambianceAudioSource.loop = true; // Set the AudioSource to loop
            ambianceAudioSource.volume *= PlayerPrefs.GetFloat("MusicVolume", 1f); // Load volume from PlayerPrefs, default to 0.8
            ambianceAudioSource.Play(); // Start playing the ambiance sound
        }
    }

    public void UpdateVolume()
    {
        if (!ambianceAudioSource)
            return;

        // Update the volume of the ambiance sound based on PlayerPrefs
        ambianceAudioSource.volume = initialVolume * PlayerPrefs.GetFloat("MusicVolume", 1f);
    }
}

