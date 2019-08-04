using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    private static SoundPlayer Singleton;

    public AudioSource StandardAudioSource;
    public List<AudioSource> PitchAdjustedAudioSources;
    public AudioSource BoomingAudioSource;
    static bool Muted { get; set; } = false;

    public static void MuteSound()
    {
        Muted = true;
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void PlaySound(AudioClip sound, float volume = .5f)
    {
        if (Muted)
        {
            return;
        }

        Singleton.StandardAudioSource.PlayOneShot(sound, volume);
    }

    public static void PlayPitchAdjustedSound(AudioClip sound, float volume = .5f)
    {
        if (Muted)
        {
            return;
        }

        Singleton.PitchAdjustedAudioSources[Random.Range(0, Singleton.PitchAdjustedAudioSources.Count)].PlayOneShot(sound, volume);
    }

    public static void PlayBoomingSound(AudioClip sound, float volume = .5f)
    {
        if (Muted)
        {
            return;
        }

        Singleton.BoomingAudioSource.PlayOneShot(sound, volume);
    }
}
