using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class BossLifeBarScript : MonoBehaviour
{
    private GameObject lifeBarParent;

    public float maxLife = 40; // maximo de vida possivel
    private float life = 0; // total de vida
    private float filler = 30; // valor pelo qual a vida sera multiplicada
    private float ghost = 0; // ghost da barra de vida
    private int barHeight = 17; // altura da barra de vida
    public Animator bossAnim;

    public Image lifeBar; // barra de vida verdadeira
    public Image lifeGhost; // ghost da barra de vida

    private float lastTime;
    private float waitTime = 1.5f;

    [HideInInspector]
    public bool fillBossLifeBar = false;

    private void Start()
    {
        lifeBarParent = this.transform.parent.gameObject;
        lifeBarParent.GetComponent<CanvasGroup>().alpha = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            UpdateLife(-2);
        }
    }

    private void FixedUpdate()
    {
        if (fillBossLifeBar)
        {
            lifeBarParent.GetComponent<CanvasGroup>().alpha = 1;
            life = Mathf.Lerp(life, maxLife, 1 * Time.deltaTime);
            lifeBar.rectTransform.sizeDelta = new Vector2(life * filler, barHeight);
            if (life >= maxLife-0.5f)
            {
                fillBossLifeBar = false;
                life = maxLife;
                lifeBar.rectTransform.sizeDelta = new Vector2(maxLife * filler, barHeight);
            }
        }

        if (life > ghost) // caso a vida seja maior que o ghost, ambos passam a ter o mesmo tamanho
        {
            ghost = life;
            lifeGhost.rectTransform.sizeDelta = new Vector2(ghost * filler, barHeight);
        }

        if ((Time.time > lastTime + waitTime) && ghost > life) // espera um pouco e diminui o ghost ate chegar na vida
        {
            ghost -= 0.1f;
            lifeGhost.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(ghost, life, 8 * Time.deltaTime) * filler, barHeight);
        }
    }

    public void UpdateLife(float amount)
    {
        if (amount < 0) // caso esteja decrementando a vida
        {
            lastTime = Time.time;
        }

        life += amount; // realiza a mudanca na vida

        if (life > maxLife) life = maxLife; // garante que ela nao seja maior que o permitido
        if (life < 0) life = 0;// garante que ela nao seja menor que o permitido

        if (life == 0 && !bossAnim.GetBool("Dead")) // mata o jogador caso ainda nao tenha feito
        {
            Die();
        }

        lifeBar.rectTransform.sizeDelta = new Vector2(life * filler, barHeight); // atualiza o tamanho da barra de vida
    }



    private void Die()
    {

    }

}
