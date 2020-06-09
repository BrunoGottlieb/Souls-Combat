using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFarSword : MonoBehaviour
{
    private Transform player;
    private Transform botPosition;

    public AudioSource swingSource;

    private float beginTime;
    private float lerpVelocity = 10f;
    private DamageDealer damageDealer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        botPosition = player.transform.Find("Bot position").transform;
        beginTime = Time.time;
        damageDealer = this.GetComponentInChildren<DamageDealer>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.parent.position = Vector3.Lerp(this.transform.position, botPosition.position, lerpVelocity * Time.deltaTime);
        //this.transform.parent.position = botPosition.position;
        if (Time.time - beginTime >= 1.5f)
        {
            lerpVelocity = 100;
        }
        this.transform.parent.rotation = player.rotation;
    }

    private void PlaySwing()
    {
        swingSource.Play();
    }

    private void SetDamageOn()
    {
        damageDealer.damageOn = true;
    }

    private void SetDamageOff()
    {
        damageDealer.damageOn = false;
    }

}
