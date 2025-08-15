using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Deals with sounds effects that are not tied to a specific object or level.
/// Also handles the volumes of all sound effects in the game.
/// </summary>
public class StaticSFX : MonoBehaviour
{
    public static StaticSFX instance { get; private set; }

    #region Inspector Variables
    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource; // AudioSource to play static sound effects
    [SerializeField] private AudioClipPlus respawnSound; // Sound to play when the player respawns
    #endregion

    #region Private Variables
    private List<AudioSourcePlus> sfxSources; // List of sfx audio sources to manage multiple audio sources
    #endregion

    #region Unity Events
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

        sfxSources = new List<AudioSourcePlus>();
    }

    private void Start()
    {
        // Initialize the audio source if it is set in the inspector
        if (audioSource == null)
        {
            Debug.LogError("StaticSFX: AudioSource is missing from " + this.name);
        }
        else
        {
            InitialiseNewSource(audioSource);
        }
    }
    #endregion

    #region Methods
    /// <summary>
    /// Plays the respawn sound effect.
    /// </summary>
    public void PlayRespawnSound()
    {
        // If no respawn sound is set, return early
        if (!audioSource)
            return;

        StartCoroutine(PlayDelayed(respawnSound));
    }

    /// <summary>
    /// Initialises a new AudioSourcePlus with the provided AudioSource.
    /// </summary>
    public void InitialiseNewSource(AudioSource newSource)
    {
        // If no AudioSource is provided
        if (!newSource)
            return;

        for (int i = sfxSources.Count - 1; i >= 0; i--)
        {
            // Remove any AudioSourcePlus entries that already are saved
            if (sfxSources[i].source == newSource)
            {
                Debug.LogWarning("StaticSFX: AudioSource already exists in sfxSources.");
                sfxSources.RemoveAt(i);
            }
        }

        // Add the new AudioSource to the list of sfxSources
        AudioSourcePlus initAudio = new AudioSourcePlus(newSource, newSource.volume);
        sfxSources.Add(initAudio);

        newSource.loop = false; // Ensure the new AudioSource does not loop
        newSource.volume *= PlayerPrefs.GetFloat("SFXVolume", 1f); // Load volume from PlayerPrefs, default to 1
    }

    /// <summary>
    /// Updates all the volumes of the sound effects based on the PlayerPrefs value for SFXVolume.
    /// Also clears out any AudioSourcePlus entries that have a null source.
    /// </summary>
    public void UpdateVolumes()
    {
        // Iterate through all AudioSourcePlus entries and update their volumes based on their initial volume
        for (int i = sfxSources.Count - 1; i >= 0; i--)
        {
            AudioSourcePlus audioSourcePlus = sfxSources[i];
            if (audioSourcePlus.source)
            {
                audioSourcePlus.source.volume = audioSourcePlus.initialVolume * PlayerPrefs.GetFloat("SFXVolume", 1f);
            }
            else
            {
                sfxSources.RemoveAt(i); // Remove AudioSourcePlus if the source is null
            }
        }
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Delays the playback of an AudioClipPlus by its specified delay time.
    /// </summary>
    private IEnumerator PlayDelayed(AudioClipPlus audio)
    {
        // Check if the audio clip is set
        if (!audio.clip)
            yield break;

        // Wait for the specified delay before playing the audio
        yield return new WaitForSeconds(audio.delay);
        audioSource.PlayOneShot(audio.clip);
    }
    #endregion

    #region Sub Classes
    /// <summary>
    /// Additional variables for AudioClip to include a delay before playing.
    /// </summary>
    [Serializable]
    public class AudioClipPlus
    {
        public AudioClip clip;
        public float delay;

        public AudioClipPlus(AudioClip clip, float delay = 0f)
        {
            this.clip = clip;
            this.delay = delay;
        }
    }

    [Serializable]
    /// <summary>
    /// Additional variables for AudioSource to include initial volume.
    /// </summary>>
    public class AudioSourcePlus
    {
        public AudioSource source;
        public float initialVolume;
        public AudioSourcePlus(AudioSource source, float initialVolume = 1f)
        {
            this.source = source;
            this.initialVolume = initialVolume;
        }
    }
    #endregion
}
