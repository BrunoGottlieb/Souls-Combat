using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public AudioSource creditsSource;
    public Text totalTimeText;
    public Text totaldeathsText;

    private void CreditsEnd() // Chamado pelo final da animacao dos creditos, vai pro menu
    {
        SceneManager.LoadScene(0);
    }

    private void SetCreditTexts()
    {
        int totalTime = (int)(PlayerPrefs.GetInt("TotalTime") + Time.time); // pega a ultima atualizacao do tempo gasto

        int hours = totalTime / 3600;
        int minutes = (totalTime % 3600) / 60;
        int seconds = (totalTime % 3600) % 60;

        totalTimeText.text = "Total time spent: " + hours.ToString("00") + ":" + minutes.ToString("00") + ":" + seconds.ToString("00");
        totaldeathsText.text = "Total deaths: " + PlayerPrefs.GetInt("DeathCount").ToString();
    }

    private void PlayCreditsMusic()
    {
        if(PlayerPrefs.GetInt("IsMusicOn") == 1)
            creditsSource.Play();
    }
}
