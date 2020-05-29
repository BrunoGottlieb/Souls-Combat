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
        Animator otherAnim = other.transform.GetComponentInParent<Animator>();
        print(other.gameObject.name + " | " + other.gameObject.tag);
        if ((otherAnim.GetComponent<Animator>() != null && otherAnim.GetBool("Attacking") || otherAnim.GetBool("Dodge")) || other.gameObject.name.Contains("Magic"))
        {
            Destroy();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "GreatSword")
       {
            Destroy();
       }
    }

    private void Destroy()
    {
        PlayDestructionSound();
        Vector3 scale = this.transform.localScale;
        Instantiate(destroyedObj, transform.position, transform.rotation);
        destroyedObj.transform.localScale = scale;
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
        audioSource.Play();
        Destroy(audioSource, 3);
    }

}
