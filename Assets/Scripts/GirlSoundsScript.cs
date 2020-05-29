using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlSoundsScript : MonoBehaviour
{
    public AudioClip heavySwordAttack; // botao direito do mouse
    public AudioClip swordAttack; // ataque principal
    public AudioClip secondHit; // segundo ataque do combo do botao esquerdo do mouse
    public AudioClip thirdHit; // terceiro ataque do combo do botao esquerdo do mouse

    public AudioClip dogeRoll; // som de rolar
    public AudioClip standingUp; // som de se levantar do chao
    public AudioClip fallOnGround; // caindo no chao
    public AudioClip reachWeapon; // sacando a arma
    public AudioClip[] footStep; // sons dos passos
    public AudioClip[] takeDamage; // personagem foi atingida

    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    public void PlaySwordAttack()
    {
        CreateAndPlay(swordAttack, 1);
    }

    public void PlayHeavySwordAttack()
    {
        CreateAndPlay(heavySwordAttack, 1);
    }

    public void PlaySecondHit()
    {
        CreateAndPlay(secondHit, 1);
    }

    public void PlayThirdHit()
    {
        CreateAndPlay(thirdHit, 1);
    }

    public void PlayDodgeRoll()
    {
        CreateAndPlay(dogeRoll, 1);
    }

    public void PlayFootStep()
    {
        if(!anim.GetBool("Intangible") && !anim.GetBool("Attacking"))
            CreateAndPlay(footStep[Random.Range(0, footStep.Length)], 0.5f, 0.5f);
    }

    public void PlayStandindUp()
    {
        CreateAndPlay(standingUp, 1);
    }

    public void PlayTakeDamage()
    {
        CreateAndPlay(takeDamage[Random.Range(0, takeDamage.Length)], 1);
    }

    public void PlayFallOnGround()
    {
        CreateAndPlay(fallOnGround, 1);
    }

    public void ReachWeapon()
    {
        CreateAndPlay(reachWeapon, 1);
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = 1;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
