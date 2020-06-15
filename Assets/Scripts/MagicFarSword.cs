using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFarSword : MonoBehaviour
{
    private Transform player;
    private Transform botPosition;

    public AudioSource swingSource;

    public Material render_material;

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

    private void OnEnable()
    {
        render_material.SetFloat("_Metallic", 0.6f);
        Color32 col = render_material.GetColor("_Color");
        col.a = 255;
        render_material.SetColor("_Color", col);
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

    private void FadeMaterial()
    {
        StartCoroutine(FadeMat());
    }

    IEnumerator FadeMat()
    {
        while(render_material.GetFloat("_Metallic") > 0|| render_material.GetColor("_Color").a > 0)
        {
            render_material.SetFloat("_Metallic", render_material.GetFloat("_Metallic") - 0.1f);
            Color32 col = render_material.GetColor("_Color");
            col.a -= 20;
            render_material.SetColor("_Color", col);
            yield return new WaitForSeconds(0.1f);
        }
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
