using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    private EventSystem eventSystem; // referencia ao event system
    //public UnityEvent onTransition;

    [Header("Audio")]
    public AudioSource pressedBtnSource; // source que toca o som de click
    public AudioSource selectedBtnSource; // referencia ao audio source que toca os sons

    [Header("Transition")]
    public GameObject screenBeforeTransition;
    public GameObject screenAfterTransition;

    private void Start()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
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
