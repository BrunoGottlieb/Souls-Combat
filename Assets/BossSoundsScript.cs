using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundsScript : MonoBehaviour
{
    public AudioClip swordSwing;
    public AudioClip[] takeDamage;

    public void PlaySwordSwing()
    {
        CreateAndPlay(swordSwing, 2);
    }

    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage[Random.Range(0, takeDamage.Length)], 2);
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
