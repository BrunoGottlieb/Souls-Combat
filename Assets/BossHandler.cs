using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandler : MonoBehaviour
{
    public Transform model;
    private BossAttackHandle bossAttackHandle;
    private Transform player;
    private Animator anim;

    private float rotationSpeed = 1;
    private bool lookAtPlayer = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        bossAttackHandle = this.GetComponent<BossAttackHandle>();
        anim = model.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.N))
        {
            anim.SetTrigger("TakeDamage");
            lookAtPlayer = false;
        }
    }

    private void HandleRotation()
    {
        if(lookAtPlayer)
            model.transform.LookAt(player.transform.position); // Mantem o boss olhando para o player
    }
    private void NearAttack()
    {
        int rand = Random.Range(0, 3);

        if(rand == 0)
        {
            anim.SetTrigger("SwordAttack");
        } else if (rand == 1)
        {
            anim.SetTrigger("JumpAttack");
        } else if (rand == 2)
        {
            anim.SetTrigger("ComboAttack");
        }
    }

    private void FarAttack()
    {
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        anim.SetBool("Move", false);
        int rand = Random.Range(0, 3);

        if (rand == 0)
        {
            anim.SetTrigger("SlideAttack");
        }
        else if (rand == 1)
        {
            anim.SetTrigger("EarthShatter");
            bossAttackHandle.EarthShatter();
        }
        else if (rand == 2)
        {
            int r = Random.Range(0, 2);
            if(r == 0)
                anim.SetFloat("Horizontal", -1);
            else
                anim.SetFloat("Horizontal", 1);
            anim.SetBool("Move", true);
        }
    }

    IEnumerator HandleReadyToAttack()
    {
        anim.SetBool("ReadyToAttack", false);
        yield return new WaitForSeconds(Random.Range(2, 4));
        anim.SetBool("ReadyToAttack", true);
    }

    private void HandleMovement()
    {
        float distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player

        if(distance < 4 && !anim.GetBool("Attacking") && anim.GetBool("ReadyToAttack")) // caso esteja proximo ao jogador
        {
            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);
            anim.SetBool("Move", false);
            NearAttack();
            StartCoroutine(HandleReadyToAttack());
        }
        else if (distance > 4 && distance < 10 && anim.GetBool("ReadyToAttack")) // caso esteja longe do jogador, vai ate ele
        {
            anim.SetFloat("Vertical", 1);
            anim.SetFloat("Horizontal", 0);
            anim.SetBool("Move", true);
            
        }
        else if (distance > 10 && anim.GetBool("ReadyToAttack") && !anim.GetBool("Attacking"))
        {
            FarAttack();
            StartCoroutine(HandleReadyToAttack());
        }
        else if (!anim.GetBool("Move"))// idle
        {
            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);
            anim.SetBool("Move", false);
        }
    }

}
