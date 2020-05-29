using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Transform model;
    public Transform greatSword;

    private Animator anim;
    public Transform player;

    public AudioClip takeDamageSound;

    void Start()
    {
        anim = model.GetComponent<Animator>();
    }

    void Update()
    {
        /*
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle") && !anim.GetBool("Attacking")) 
        {
            Vector3 rotationOffset = player.transform.position - model.position;
            rotationOffset.y = 0;
            float lookDirection = Vector3.SignedAngle(model.forward, rotationOffset * 10, Vector3.up);
            anim.SetFloat("LookDirection", lookDirection);
        } else if (!anim.GetBool("Attacking"))
        {
            model.transform.LookAt(player.transform.position); // olha para o player caso nao esteja atacando
        }
        */

        if (!anim.GetBool("Attacking"))
        {
            model.transform.LookAt(player.transform.position); // olha para o player caso nao esteja atacando
        }

        model.transform.eulerAngles = new Vector3(0, model.transform.eulerAngles.y, 0);
        //model.transform.position = new Vector3(model.transform.position.x, 0, model.transform.position.z);

        GreatSwordCollider(anim.GetBool("Attacking")); // seta a GreatSword com collider on enquanto estiver atacando

    }

    private void GreatSwordCollider(bool b) // seta a GreatSword com collider on enquanto estiver atacando
    {
        greatSword.GetComponent<BoxCollider>().isTrigger = !b;
    }

    private void OnTriggerEnter(Collider other) // player atacou o boss e o boss nao esta atacando
    {
        if(other.gameObject.tag == "Sword" && other.gameObject.GetComponentInParent<Animator>().GetBool("Attacking") && !anim.GetBool("Attacking") && !anim.GetBool("TakingDamage"))
        {
            CreateAndPlay(takeDamageSound, 2); // som de dano
            anim.SetTrigger("TakeDamage"); // animacao de dano
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.transform.GetComponentInChildren<Animator>().GetBool("Attacking") && !anim.GetBool("Attacking") && !anim.GetBool("TakingDamage"))
        {
            CreateAndPlay(takeDamageSound, 2);
            anim.SetTrigger("TakeDamage");
            collision.gameObject.GetComponent<Rigidbody>().isKinematic = true;
            //capsuleCol.isTrigger = true;
            //rb.isKinematic = true;
            //anim.SetBool("Intangible", true);
        }
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f) // toca um som e o destroi deopis de x segundos
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }

}
