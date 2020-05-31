using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private Transform player;
    private float speed = 50;
    private float turn = 20;
    private Vector3 offset;

    private float distance;
    private Rigidbody rb;

    private bool chase = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        offset = new Vector3(0, 1f, 0);
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = (player.transform.position - this.transform.position).sqrMagnitude;

        rb.velocity = transform.forward * speed; // aplica velocidade ao projetil

        if(distance > 2 && chase) // caso ainda nao tenha passado pelo player
        {
            Quaternion targetRotation = Quaternion.LookRotation((player.position + offset) - transform.position);
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, targetRotation, turn));
        } else
        {
            chase = false; // nao persegue mais o player, apenas segue em frente
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(this.gameObject, 0.05f); // destroi este objeto apos colidir com algo
    }
}
