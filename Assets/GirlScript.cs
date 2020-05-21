﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlScript : MonoBehaviour
{
    public Transform model;

    private float moveSpeed = 4;
    private Animator anim;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private CapsuleCollider capsuleCol;
    private Rigidbody rb;

    public AudioClip swordDamageSound;

    void Start()
    {
        anim = model.GetComponent<Animator>();
        mainCamera = Camera.main;
        capsuleCol = model.GetComponentInChildren<CapsuleCollider>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (anim.GetBool("Equiped")) moveSpeed = 4;
        else moveSpeed = 5;

        Move();
        Rotation();
        Attack();

    }

    private void Move()
    {
        float x = mainCamera.transform.TransformDirection(stickDirection).x;
        float z = mainCamera.transform.TransformDirection(stickDirection).z;
        if (x > 1) x = 1; // assegura que o jogador nao ira se mover mais rapido em diagonal
        if (z > 1) z = 1;

        if (anim.GetBool("CanMove"))
        {
            model.position += new Vector3(x * moveSpeed * Time.deltaTime, 0, z * moveSpeed * Time.deltaTime); // move o jogador para frente
            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1).magnitude, 0.02f, Time.deltaTime); // clamp para limitar a 1, visto que a diagonal seria de 1.4
        }

        //anim.SetFloat("Horizontal", stickDirection.x);
        //anim.SetFloat("Vertical", stickDirection.z);
    }

    private void Rotation()
    {
        if (anim.GetBool("Attacking") || !anim.GetBool("CanMove")) return;
        Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection) * 1.5f;
        rotationOffset.y = 0;
        model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 20f);
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !anim.GetBool("Attacking") && anim.GetBool("Equiped")) // ataque primario
        {
            anim.SetTrigger("LightAttack");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !anim.GetBool("Attacking") && anim.GetBool("Equiped")) // ataque secundario
        {
            anim.SetTrigger("HeavyAttack");
        }

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("Attacking")) // rola caso nao esteja atacando
        {
            anim.SetTrigger("Dodge");
        }

        if (Input.GetKeyDown(KeyCode.Mouse2)) // botao do meio do mouse
        {
            anim.SetTrigger("Weapon");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetBool("TargetLockMode", true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name + " colidiu comigo");
        if(collision.gameObject.tag == "GreatSword" && collision.transform.GetComponentInChildren<Animator>().GetBool("Attacking") && !anim.GetBool("Intangible"))
        {
            CreateAndPlay(swordDamageSound, 2);
            anim.SetTrigger("TakeDamage");
            capsuleCol.isTrigger = true;
            rb.isKinematic = true;
            anim.SetBool("Intangible", true);
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
