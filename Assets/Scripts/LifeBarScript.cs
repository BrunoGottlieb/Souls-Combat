using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class LifeBarScript : MonoBehaviour
{
    private float life = 10; // total de vida
    private float ghost = 10; // total de vida
    public Animator girlAnim;
    public Text lifeText; // texto debug

    public Image lifeBar; // barra de vida verdadeira
    public Image lifeGhost; // ghost da barra de vida

    // Estus Flask
    public int estusFlask = 5; // quantidade de estus disponivel
    public Text estusFlaskText; // texto que informa a quantia de estus disponivel

    private float lastTime;
    private float waitTime = 1.5f;

    public GameObject youDiedScreen;
    private ColorGrading colorGradingLayer = null;

    private void Start()
    {
        estusFlaskText.text = estusFlask.ToString();
        lifeBar.rectTransform.sizeDelta = new Vector2(life * 100, 25);
        lifeGhost.rectTransform.sizeDelta = new Vector2(life * 100, 25);

        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGradingLayer);
    }

    private void FixedUpdate()
    {
        if (life > ghost) // caso a vida seja maior que o ghost, ambos passam a ter o mesmo tamanho
        {
            ghost = life;
            lifeGhost.rectTransform.sizeDelta = new Vector2(ghost * 100, 25);
        }

        if (Time.time > lastTime + waitTime && ghost > life) // espera um pouco e diminui o ghost ate chegar na vida
        {
            ghost -= 0.1f;
            lifeGhost.rectTransform.sizeDelta = new Vector2(Mathf.Lerp(ghost, life, 5 * Time.deltaTime) * 100, 25);
        }

        if (girlAnim.GetBool("Dead"))
        {
            colorGradingLayer.saturation.value = Mathf.Lerp(colorGradingLayer.saturation.value, -100, 1 * Time.deltaTime);
            youDiedScreen.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(youDiedScreen.GetComponent<CanvasGroup>().alpha, 1, 0.5f * Time.deltaTime);
        }
    }

    public void UpdateLife(float amount)
    {
        if (amount < 0) // caso esteja decrementando a vida
        {
            lastTime = Time.time;
        } else
        {
            estusFlask -= 1; // diminui 1 estus na quantia disponivel
            estusFlaskText.text = estusFlask.ToString();
        }

        life += amount;

        if (life > 10) life = 10;
        if (life < 0) life = 0;

        if (life == 0 && !girlAnim.GetBool("Dead"))
        {
            Die();
        }

        lifeText.text = life.ToString("0.0");
        lifeBar.rectTransform.sizeDelta = new Vector2(life * 100, 25); // atualiza o tamanho da barra de vida
    }

    private void Die()
    {
        girlAnim.SetTrigger("DieForward");
        girlAnim.SetBool("Dead", true);
        youDiedScreen.SetActive(true);
        girlAnim.gameObject.GetComponent<IKFootPlacement>().SetIntangibleOn();
    }

}
