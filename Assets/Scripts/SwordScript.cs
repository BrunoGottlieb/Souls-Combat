using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordScript : MonoBehaviour
{
    public Animator girlAnim;
    public AudioClip[] sparkSound; // som quando a espada bate em algo
    private AudioSource sparksSource; // audioSource que ira tocar o som, aleatoriamente
    public GameObject sparkEffect;

    private void Start()
    {
        sparksSource = this.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 13 && girlAnim.GetBool("Attacking") && !sparksSource.isPlaying) // caso seja um elemento do cenario
        {
            sparksSource.clip = sparkSound[Random.Range(0, sparkSound.Length)];
            sparksSource.Play();
            if (other.transform.parent.gameObject.name.Contains("Rochas")) // caso seja uma rocha, instancia o efeito
            {
                GameObject spark = Instantiate(sparkEffect, this.transform.position, Quaternion.identity);
                Destroy(spark, 0.3f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("Colidiu");
        GameObject spark = Instantiate(sparkEffect, collision.contacts[0].point, Quaternion.identity);
        Destroy(spark, 1);
    }

}
