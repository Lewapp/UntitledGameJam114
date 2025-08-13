using System;
using System.Collections;
using UnityEngine;

public class StaticSFX : MonoBehaviour
{
    public static StaticSFX instance { get; private set; }

    [Header("Sound Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClipPlus respawnSound;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // Ensure the GameObject is not destroyed on scene load
        DontDestroyOnLoad(gameObject);
    }

    public void PlayRespawnSound()
    {
        if (!audioSource)
            return;

        StartCoroutine(PlayDelayed(respawnSound));
    }

    private IEnumerator PlayDelayed(AudioClipPlus audio)
    {
        if (!audio.clip)
            yield break;

        yield return new WaitForSeconds(audio.delay);

        audioSource.PlayOneShot(audio.clip);
    }

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
}
