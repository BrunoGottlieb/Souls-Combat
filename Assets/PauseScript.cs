using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public Material blur;
    public GameObject menuzinho;
    private Animator anim;
    private bool lerpDone;
    private float enabledTime;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        print("Start");
    }

    private void OnEnable()
    {
        print("Enable");
        lerpDone = false;
        StartCoroutine(BlurLerpOn(0));
        enabledTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.GetPauseInput() && lerpDone)
        {
            anim.SetTrigger("Close");
            StartCoroutine(BlurLerpOff(1.5f));
        }

        if (Time.time - enabledTime >= 0.4f)
        {
            menuzinho.SetActive(true);
        }
    }

    IEnumerator BlurLerpOn(float value)
    {
        blur.SetFloat("_Size", value);
        yield return new WaitForSeconds(0.1f);
        if(value < 1.5f)
        {
            StartCoroutine(BlurLerpOn(value + 0.25f));
        } else
        {
            lerpDone = true;
        }
    }

    IEnumerator BlurLerpOff(float value)
    {
        menuzinho.SetActive(false);
        blur.SetFloat("_Size", value);
        yield return new WaitForSeconds(0.1f);
        if (value > 0)
        {
            StartCoroutine(BlurLerpOff(value - 0.25f));
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

}
