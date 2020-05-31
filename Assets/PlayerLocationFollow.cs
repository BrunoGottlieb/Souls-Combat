using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocationFollow : MonoBehaviour
{
    public Transform player;
    public Transform boss;
    public Vector3 offset;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.position = player.position + offset;
        this.transform.LookAt(boss.position);
    }
}
