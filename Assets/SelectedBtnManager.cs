using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedBtnManager : MonoBehaviour
{
    public GameObject selectedBtn; // botao que estara selecionado quando esta tela ativar

    private EventSystem eventSystem; // referencia ao event system

    private void OnEnable()
    {
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
        StartCoroutine(HighlightBtn());
    }

    IEnumerator HighlightBtn()
    {
        yield return new WaitForEndOfFrame();
        eventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        eventSystem.SetSelectedGameObject(selectedBtn);
    }

}
