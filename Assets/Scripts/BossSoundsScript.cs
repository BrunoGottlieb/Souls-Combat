using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundsScript : MonoBehaviour
{
    public AudioClip swordSwing;
    public AudioClip fastSwing;
    public AudioClip longSwordSwing;
    public AudioClip reachGreatSword;
    public AudioClip[] takeDamage;

    public void PlaySwordSwing()
    {
        CreateAndPlay(swordSwing, 2);
    }

    public void PlayLongSwordSwing()
    {
        CreateAndPlay(longSwordSwing, 3);
    }

    public void PlayFastSwordSwing()
    {
        CreateAndPlay(fastSwing, 1);
    }

    public void PlayReachGreatSword()
    {
        CreateAndPlay(reachGreatSword, 2, 1, 20);
    }

    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage[Random.Range(0, takeDamage.Length)], 2);
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f, float minDistance = 15f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = minDistance;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
