using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [Header("Super Main Menu")]
    public GameObject pressAnyButton;
    private CanvasGroup pressAnyBtnCanvasGroup;

    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject buttonsMenu;

    [Header("Tutorial")]
    public GameObject tutorial;

    [Header("Audio")]
    public AudioSource startSound;

    private bool pressedAnyBtn;

    private void Start()
    {
        GetReferences();
        SetObjects();
    }
    private void GetReferences()
    {
        pressAnyBtnCanvasGroup = pressAnyButton.GetComponent<CanvasGroup>();
    }


    private void Update()
    {
        if (!pressedAnyBtn && pressAnyButton.activeSelf)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    OnPressedStart();
                    pressedAnyBtn = true;
                }
            }
        }
    }

    
    private void SetObjects()
    {
        pressAnyButton.SetActive(true);
        buttonsMenu.SetActive(false);
        mainMenu.SetActive(true);
        tutorial.SetActive(false);
    }

    public void OnPressedStart()
    {
        StartCoroutine(FadeStartButton());
    }

    IEnumerator FadeStartButton()
    {
        startSound.Play();
        while (pressAnyBtnCanvasGroup.alpha > 0)
        {
            pressAnyBtnCanvasGroup.alpha -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        pressAnyButton.SetActive(false);
        buttonsMenu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
