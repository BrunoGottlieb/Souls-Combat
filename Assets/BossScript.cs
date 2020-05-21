using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Transform model;
    public Transform greatSword;

    private Animator anim;
    public Transform player;

    private BossAttack attackScript;

    public AudioClip takeDamageSound;

    void Start()
    {
        anim = model.GetComponent<Animator>();
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        attackScript = this.GetComponent<BossAttack>();
    }

    void Update()
    {
        if (!anim.GetBool("Attacking"))
        {
            //model.transform.LookAt(player.transform.position);
            Vector3 rotationOffset = player.transform.position - model.position;
            rotationOffset.y = 0;
            float lookDirection = Vector3.SignedAngle(model.forward, rotationOffset, Vector3.up);
            anim.SetFloat("LookDirection", lookDirection);
            print("LD: " + lookDirection);
        }

        model.transform.eulerAngles = new Vector3(0, model.transform.eulerAngles.y, 0);
        //model.transform.position = new Vector3(model.transform.position.x, 0, model.transform.position.z);

        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetTrigger("Attack");
            //attackScript.Saltar();
        }

        GreatSwordCollider(anim.GetBool("Attacking"));

        //Move();
    }

    private void GreatSwordCollider(bool b)
    {
        greatSword.GetComponent<BoxCollider>().isTrigger = !b;
    }


    private void Move()
    {
        float distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player
        if (distance < 4 && !anim.GetBool("Attacking")) // caso esteja proximo ao jogador
        {
            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);
        } else if (distance > 4 && distance < 10)
        {
            anim.SetTrigger("Attack");
            anim.SetFloat("Vertical", 0);

        } else if (distance > 10)
        {
            anim.SetFloat("Vertical", 1);
            anim.SetFloat("Horizontal", 0);
        }
    }

    private void NearAttack()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.transform.GetComponentInChildren<Animator>().GetBool("Attacking") && !anim.GetBool("Attacking"))
        {
            CreateAndPlay(takeDamageSound, 2);
            anim.SetTrigger("TakeDamage");
            //capsuleCol.isTrigger = true;
            //rb.isKinematic = true;
            //anim.SetBool("Intangible", true);
        }
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }

}
