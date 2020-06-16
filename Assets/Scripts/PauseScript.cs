using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public Material blur;
    public GameObject insideMenuzinho;
    public GameObject confirmationScreen;
    public GameObject achievementScreen;
    private Animator anim;
    private bool lerpDone;
    public AudioSource transitionSource;
    public AudioSource selectSource;

    [Header("Configuration Screen")]
    public GameObject configurationScreen;

    [Header("Achievements")]
    public GameObject[] achievements;

    void Start()
    {
        anim = insideMenuzinho.transform.parent.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        transitionSource.Play();
        GameManagerScript.gameIsPaused = true;
        GameManagerScript.HideCursor(false);
        lerpDone = false;
        insideMenuzinho.transform.parent.gameObject.SetActive(true);
        insideMenuzinho.SetActive(false);
        StartCoroutine(BlurLerpOn(0));
        foreach (GameObject ach in achievements) ach.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetPauseInput() && lerpDone)
        {
            CloseMenu();
        }
    }

    public void CloseMenu()
    {
        insideMenuzinho.gameObject.SetActive(true);
        confirmationScreen.SetActive(false);
        achievementScreen.SetActive(false);
        configurationScreen.SetActive(false);
        selectSource.Play();
        anim.SetTrigger("Close");
        transitionSource.Play();
        foreach (GameObject ach in achievements) ach.SetActive(false);
        StartCoroutine(BlurLerpOff(1.5f));
    }

    IEnumerator BlurLerpOn(float value)
    {
        blur.SetFloat("_Size", value);
        yield return new WaitForSeconds(0.1f);
        if(value < 1.5f)
        {
            if(value < 0.5f) insideMenuzinho.SetActive(true);
            StartCoroutine(BlurLerpOn(value + 0.25f));
        } else
        {
            lerpDone = true;
        }
    }

    IEnumerator BlurLerpOff(float value)
    {
        blur.SetFloat("_Size", value);
        yield return new WaitForSeconds(0.1f);
        if (value > 0)
        {
            if (value > 0.5f) insideMenuzinho.SetActive(false);
            StartCoroutine(BlurLerpOff(value - 0.25f));
        }
        else
        {
            GameManagerScript.HideCursor(true);
            GameManagerScript.gameIsPaused = false;
            this.gameObject.SetActive(false);
        }
    }

    // Configuration Screen

    public void OpenConfigurationScreen()
    {
        selectSource.Play();
        configurationScreen.SetActive(true);
        transitionSource.Play();
    }

    public void OpenAchievementsScreen()
    {
        selectSource.Play();
        achievementScreen.SetActive(true);
        transitionSource.Play();
    }

    public void ExitBtn() // Chamado ao clicar no botao de Exit Game
    {
        selectSource.Play();
        transitionSource.Play();
        confirmationScreen.SetActive(true);
    }

    public void Exit() // Realmente sai do jogo
    {
        Application.Quit();
    }

}
