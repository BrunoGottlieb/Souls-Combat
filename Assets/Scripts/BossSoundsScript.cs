using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundsScript : MonoBehaviour
{
    public AudioClip swordSwing;
    public AudioClip fastSwing;
    public AudioClip longSwordSwing;
    public AudioClip reachGreatSword;
    public AudioClip killed;
    public AudioClip transposing;
    public AudioClip preImpact;
    public AudioClip impact;
    public AudioClip scream;
    public AudioClip[] takeDamage;
    public AudioClip[] footStep;
    public AudioClip[] greatSwordHit;
    public AudioSource footSoundSource;
    public BossLifeBarScript bossLifeBar;
    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

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
        bossLifeBar.FillBossLifeBar();
        GameObject.Find("GameManager").GetComponent<GameManagerScript>().BeginMusic();
    }

    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage[Random.Range(0, takeDamage.Length)], 2);
    }

    public void PlayKilled()
    {
        CreateAndPlay(killed, 2, 1, 20);
    }

    public void PlayTransposingSound()
    {
        CreateAndPlay(transposing, 3.5f, 1, 20);
    }

    public void PlayFootStep()
    {
        footSoundSource.transform.position = this.transform.root.GetChild(0).transform.position;
        if(!footSoundSource.isPlaying && !anim.GetBool("Dead") && !anim.GetBool("GameEnd") && anim.GetBool("CanRotate"))
            footSoundSource.Play();
        //CreateAndPlay(footStep[Random.Range(0, footStep.Length)], 1, 0.5f);
    }

    public void PlayGreatSwordHit()
    {
        CreateAndPlay(greatSwordHit[Random.Range(0, greatSwordHit.Length)], 1.5f, 1, 10);
    }

    public void PlayPreImpact()
    {
        CreateAndPlay(preImpact, 3, 1, 20);
    }

    public void PlayImpact()
    {
        CreateAndPlay(impact, 2, 1, 50);
    }

    public void PlayScream()
    {
        CreateAndPlay(impact, 3, 1, 20);
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f, float minDistance = 15f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = 50;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
