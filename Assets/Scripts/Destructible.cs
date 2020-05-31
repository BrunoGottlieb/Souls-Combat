using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedObj;
    public AudioClip destructionSound;

    private void Start()
    {
        //playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name + " colidiu com " + this.gameObject.name);
        Animator otherAnim = other.transform.root.GetComponentInChildren<Animator>();
        
        if ((otherAnim != null && otherAnim.GetBool("Attacking")) && (other.gameObject.tag == "GreatSword" || other.gameObject.tag == "Sword")) // atingido por espadas
        {
            Destroy();
        }
        else if(other.gameObject.name.Contains("Magic") || other.gameObject.tag == "Magic") // atingido por magica
        {
            Destroy();
        }
    }

    private void OnCollisionEnter(Collision collision) // colisao porque eh o corpo do boss
    {
        if (collision.gameObject.name.Contains("Boss")) // atingido pelo boss se movendo
        {
            Destroy();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Destroy();
    }

    private void Destroy()
    {
        PlayDestructionSound();
        Vector3 scale = this.transform.localScale;
        Instantiate(destroyedObj, transform.position, transform.rotation, transform.parent);
        destroyedObj.transform.localScale = Vector3.one;
        Destroy(this.gameObject);
    }

    private void PlayDestructionSound()
    {
        GameObject audio = Instantiate(new GameObject());
        audio.transform.parent = this.transform.parent;
        audio.transform.position = this.transform.position;
        AudioSource audioSource = audio.AddComponent<AudioSource>();
        audioSource.clip = destructionSound;
        audioSource.spatialBlend = 1;
        audioSource.minDistance = 40;
        audioSource.Play();
        Destroy(audioSource, 3);
    }

}
