using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSwordFromSky : MonoBehaviour
{
    public float fallSpeed = 50;
    public AudioClip hitGroundSound;
    private bool hasPlayedHitSound = false; // garante que tocara o som atingindo o chao apenas uma vez

    private void OnEnable()
    {
        this.gameObject.layer = 12;
        this.transform.GetChild(0).gameObject.layer = 12;
        Destroy(this.gameObject, 2);
        this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, Random.Range(0f, 360f), this.transform.eulerAngles.z);
    }

    void Update()
    {
        if (this.transform.position.y > 0.5f) // enquanto ainda nao tiver atingido o chao
        {
            this.transform.position += transform.up * Time.deltaTime * fallSpeed * -1; // gravidade
        } 
        else if (!hasPlayedHitSound) // para a espada ao atingir o chao e toca o som
        {
            hasPlayedHitSound = true; // garante que tocara o som atingindo o chao apenas uma vez
            CreateAndPlay(hitGroundSound, 1);
            this.GetComponent<DamageDealer>().damageOn = false; // espada nao causara mais dano ao estar no chao
        }
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
