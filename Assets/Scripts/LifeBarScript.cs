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

    public Image lifeBar; // barra de vida verdadeira
    public Image lifeGhost; // ghost da barra de vida

    // Estus Flask
    public int estusFlask = 5; // quantidade de estus disponivel
    public Text estusFlaskText; // texto que informa a quantia de estus disponivel

    private float lastTime;
    private float waitTime = 1.5f;

    public GameObject youDiedScreen;
    private ColorGrading colorGradingLayer = null;

    // Bleeding
    public GameObject bleedingParent;
    public Image bleedingBar;
    private float bleeding;

    private bool SloDownTime;
    private float journeyLength = 15;
    private float startTime = -1;

    public GameObject deathCounter;
    public GameManagerScript gameManager; // usado para reiniciar depois de morrer
    public BossLifeBarScript bossLifeManager; // usado para conferir o achievement Almost There
    public AchievementManager achievementManager;

    private void Start()
    {
        estusFlaskText.text = estusFlask.ToString();
        lifeBar.rectTransform.sizeDelta = new Vector2(life * 100, 25);
        lifeGhost.rectTransform.sizeDelta = new Vector2(life * 100, 25);

        PostProcessVolume volume = Camera.main.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings(out colorGradingLayer);

        gameManager.playerIsDead = false;
    }

    private void FixedUpdate()
    {
        if (SloDownTime && Time.timeScale > 0.5f) // player morreu, desacelerar o tempo
        {
            print("a");
            if(startTime <= 0)
                startTime = Time.time;
            float distCovered = (Time.time - startTime) * 0.1f;
            float fractionOfJourney = distCovered / journeyLength;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.5f, fractionOfJourney);   
        }

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

        if (girlAnim.GetBool("Dead")) // ativa a tela de You Died
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
        } 
        else // esta aumentado a vida
        {
            estusFlask -= 1; // diminui 1 estus na quantia disponivel
            estusFlaskText.text = estusFlask.ToString(); // atualiza a quantia de estus no icone na tela
            //canBleed = false; // para o bleeding do player
            StopAllCoroutines(); // para todos os bleedings
            bleedingParent.SetActive(false);
        }

        life += amount; // realiza a mudanca na vida

        if (life > 10) life = 10; // garante que ela nao seja maior que o permitido
        if (life < 0) life = 0;// garante que ela nao seja menor que o permitido

        if (life == 0 && !girlAnim.GetBool("Dead")) // mata o jogador caso ainda nao tenha feito
        {
            Die();
        }

        lifeBar.rectTransform.sizeDelta = new Vector2(life * 100, 25); // atualiza o tamanho da barra de vida
    }

    public void StartBleeding() // metodo chamado pelo impacto da fireball
    {
        if (IsDead()) return; // nao comeca a sangrar caso o player ja esteja morto
        bleeding = 400;
        bleedingBar.rectTransform.sizeDelta = new Vector2(bleeding, 20);
        bleedingParent.SetActive(true);
        StartCoroutine(Burning(10));
    }

    IEnumerator Burning(int cicles)
    {
        yield return new WaitForSeconds(1f);
        if(cicles > 0)
        {
            UpdateLife(-0.2f);
            bleeding -= 40;
            bleedingBar.rectTransform.sizeDelta = new Vector2(bleeding, 20);
            StartCoroutine(Burning(cicles - 1));
        } else
        {
            bleedingParent.SetActive(false);
        }
    }

    private void Die()
    {
        girlAnim.SetFloat("Vertical", 0); // para qualquer movimento do player
        girlAnim.SetFloat("Horizontal", 0);
        girlAnim.SetFloat("Speed", 0);
        girlAnim.SetTrigger("DieForward"); // animacao de morte
        girlAnim.SetBool("Dead", true); // seta o estado como morto
        youDiedScreen.SetActive(true); // ativa a tela final
        girlAnim.gameObject.GetComponent<IKFootPlacement>().SetIntangibleOn();
        bleedingParent.SetActive(false); // tira o bleeding para ele não ficar na frente da escrita

        achievementManager.TriggerFirstDeath(); // pede para o achievementManager conferir First Death

        gameManager.playerIsDead = true; // avisa o game manager de que o player esta morto

        if (bossLifeManager.GetBossLifeAmount() <= 4) achievementManager.TriggerAlmostThere(); // morreu e o boss tinha 10% ou menos de vida

        StartCoroutine(ShowDeathCounter());

        if (gameManager.isAutoRestartOn)
        {
            StartCoroutine(WaitToRestart());
        }
    }

    IEnumerator ShowDeathCounter()
    {
        yield return new WaitForSeconds(0.5f);
        int deathNum = PlayerPrefs.GetInt("DeathCount") + 1;
        PlayerPrefs.SetInt("DeathCount", deathNum);
        deathCounter.SetActive(true); // exibe o contador de mortes
        deathCounter.GetComponentInChildren<Text>().text = deathNum.ToString();

        if (deathNum == 10) achievementManager.TriggerTenDeathMark(); // achievementManager 10 deaths trigger
    }

    IEnumerator WaitToRestart()
    {
        yield return new WaitForSeconds(2);
        gameManager.Restart();
    }

    public bool IsDead()
    {
        return girlAnim.GetBool("Dead");
    }

    public bool GetNoDamageTaken()
    {
        return life == 10;
    }

    public int GetEstusFlaskAmount()
    {
        return estusFlask;
    }

}
