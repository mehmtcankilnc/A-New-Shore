using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundEffectsManager : MonoBehaviour
{
    #region Singleton
    public static SoundEffectsManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    public List<AudioClip> soundEffects;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlaySoundEffect(string soundEffect, bool isWalkingOrRunning = false)
    {
        var clip = soundEffects.Find(x => x.name == soundEffect);
        audioSource.clip = clip;

        if (isWalkingOrRunning)
        {
            audioSource.volume = 0.06f;
            audioSource.Play();
        }
        else
        {
            audioSource.volume = 1f;
            audioSource.Play();
        }

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.clip = null;
        }
    }
}
