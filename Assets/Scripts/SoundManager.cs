using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static void CreateAndPlay(AudioClip clip, GameObject parent, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = parent.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
