using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuScript : MonoBehaviour
{
    // Super main menu
    public GameObject pressAnyButton;
    private CanvasGroup pressAnyBtnCanvasGroup;

    // Main menu
    public GameObject buttonsMenu;
    public GameObject firstSelected;

    // Audio
    public AudioSource ClickSound;

    private void Start()
    {
        GetReferences();
        SetObjects();
    }

    private void GetReferences()
    {
        pressAnyBtnCanvasGroup = pressAnyButton.GetComponent<CanvasGroup>();
    }

    private void SetObjects()
    {
        pressAnyButton.SetActive(true);
        buttonsMenu.SetActive(false);
    }

    public void OnPressedStart()
    {
        StartCoroutine(FadeStartButton());
    }

    IEnumerator FadeStartButton()
    {
        ClickSound.Play();
        while (pressAnyBtnCanvasGroup.alpha > 0)
        {
            pressAnyBtnCanvasGroup.alpha -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        pressAnyButton.SetActive(false);
        buttonsMenu.SetActive(true);
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(firstSelected);
    }

    public void Exit()
    {
        Application.Quit();
    }

}
