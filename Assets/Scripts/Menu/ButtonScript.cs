using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EventSystem eventSystem; // referencia ao event system
    private GameObject gameManager; // referencia ao GameManager para ser usado pelos toggles
    private Animator anim;

    [Header("Audio")]
    public AudioSource pressedBtnSource; // source que toca o som de click
    public AudioSource selectedBtnSource; // referencia ao audio source que toca os sons

    [Header("Transition")]
    public GameObject screenBeforeTransition;
    public GameObject screenAfterTransition;

    [Header("Toggle")]
    public bool iAmToggle;
    public string playerPrefName;

    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        anim = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if (iAmToggle) // controla se o toggle esta ligado ou desligado, caso ele seja um
        {
            this.GetComponent<Toggle>().isOn = PlayerPrefs.GetInt(playerPrefName) == 1 ? true : false;
        }
    }

    public void MyToggleMethod() // metodo chamado quando o valor do toggle eh alterado
    {
        if (!Application.isPlaying) return;
        pressedBtnSource.Play();
        PlayerPrefs.SetInt(playerPrefName, this.GetComponent<Toggle>().isOn ? 1 : 0);
        if(gameManager == null) gameManager = GameObject.FindGameObjectWithTag("GameManager");
        gameManager.GetComponent<GameManagerScript>().CheckForChanges(); // aplica as mudancas
    }

    public void OnSelect(BaseEventData eventData)
    {
        selectedBtnSource.Play();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(this.gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        anim.SetTrigger("Normal");
    }

    public void OnClickTransition()
    {
        StartCoroutine(DoTransition());
    }

    IEnumerator DoTransition()
    {
        pressedBtnSource.Play();
        while (screenBeforeTransition.GetComponent<CanvasGroup>().alpha > 0)
        {
            screenBeforeTransition.GetComponent<CanvasGroup>().alpha -= 0.05f;
            yield return new WaitForSeconds(0.075f);
        }
        screenAfterTransition.SetActive(true);
        screenAfterTransition.GetComponent<CanvasGroup>().alpha = 1f;
        screenBeforeTransition.SetActive(false);
    }
}
