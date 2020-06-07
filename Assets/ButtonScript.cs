using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScript : MonoBehaviour, ISelectHandler
{
    public AudioSource selectedBtnSource;

    public void OnSelect(BaseEventData eventData)
    {
        selectedBtnSource.Play();
    }
}
