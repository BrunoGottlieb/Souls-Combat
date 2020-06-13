using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationScript : MonoBehaviour
{
    public GameObject pauseMenuzinho;
    public GameObject insideContent;
    public GameObject continueBtn;
    public AudioSource transitionSource;
    private bool closing = false;

    private void OnEnable()
    {
        closing = false;
        insideContent.SetActive(false);
        pauseMenuzinho.SetActive(false);
        continueBtn.SetActive(false);
    }

    private void EnableInsideContent()
    {
        if (!closing)
        {
            insideContent.SetActive(true);
            continueBtn.SetActive(true);
        }
        else
        {
            insideContent.SetActive(false);
            StartCoroutine(DisableConfigScreen());
            closing = false;
        }
    }

    public void ContinueBtn()
    {
        closing = true;
        this.GetComponent<Animator>().SetTrigger("Close");
        pauseMenuzinho.SetActive(true);
        transitionSource.Play();
    }

    IEnumerator DisableConfigScreen()
    {
        yield return new WaitForSeconds(0.15f);
        this.gameObject.SetActive(false);
    }

}
