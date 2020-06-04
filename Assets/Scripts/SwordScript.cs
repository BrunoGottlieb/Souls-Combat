using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Animator girlAnim;
    public AudioClip[] sparkSound; // som quando a espada bate em algo
    private AudioSource sparksSource; // audioSource que ira tocar o som, aleatoriamente

    private void Start()
    {
        sparksSource = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 13 && girlAnim.GetBool("Attacking") && !sparksSource.isPlaying) // caso seja um elemento do cenario
        {
            sparksSource.clip = sparkSound[Random.Range(0, sparkSound.Length)];
            sparksSource.Play();
        }
    }

}
