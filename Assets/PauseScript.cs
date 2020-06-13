using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public Material blur;
    public GameObject insideMenuzinho;
    private Animator anim;
    private bool lerpDone;
    public AudioSource transitionSource;

    [Header("Configuration Screen")]
    public GameObject configurationScreen;

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
        insideMenuzinho.SetActive(false);
        StartCoroutine(BlurLerpOn(0));
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
        anim.SetTrigger("Close");
        transitionSource.Play();
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
        configurationScreen.SetActive(true);
        transitionSource.Play();
    }

}
