using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlSoundsScript : MonoBehaviour
{
    public GirlRayCaster rayCaster; // rayCast disparado para o chao
    public AudioClip heavySwordAttack; // botao direito do mouse
    public AudioClip swordAttack; // ataque principal
    public AudioClip secondHit; // segundo ataque do combo do botao esquerdo do mouse
    public AudioClip thirdHit; // terceiro ataque do combo do botao esquerdo do mouse
    public AudioClip killed; // som tocado no comeco da animacao de morte
    public AudioClip dogeRoll; // som de rolar
    public AudioClip standingUp; // som de se levantar do chao
    public AudioClip fallOnGround; // caindo no chao
    public AudioClip reachWeapon; // sacando a arma
    public AudioClip[] footStepSand; // sons dos passos na areia
    public AudioClip[] footStepStone; // sons dos passos nas pedras
    public AudioClip[] takeDamage; // personagem foi atingida
    public AudioSource footSource; // audiosource que toca o som dos passos

    public GameObject bonfireLit; // pai do bonfire aceso

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
        if(!anim.GetBool("Intangible") && !anim.GetBool("Attacking") && !anim.GetBool("Dead") && !anim.GetBool("Dodging") && anim.GetBool("CanMove"))
        {
            //if (!footSource.isPlaying)
            {
                if(!rayCaster.gameObject.activeSelf || rayCaster.IamOver == "Sand")
                {
                    footSource.volume = 0.25f;
                    footSource.PlayOneShot(footStepSand[Random.Range(0, footStepSand.Length)]);
                }
                else if (rayCaster.IamOver == "Stone")
                {
                    footSource.volume = 0.5f;
                    footSource.PlayOneShot(footStepStone[Random.Range(0, footStepStone.Length)]);
                }
            }
        }
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

    public void PlayKilled()
    {
        CreateAndPlay(killed, 2, 1);
    }

    public void LightBonFire()
    {
        bonfireLit.SetActive(true);
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.minDistance = 10;
        audioSource.maxDistance = 50;
        audioSource.spatialBlend = 1;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }
}
