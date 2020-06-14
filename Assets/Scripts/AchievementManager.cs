using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public GameObject firstDeath;
    public GameObject tenDeathMark;
    public GameObject iAmLearning;
    public GameObject almostThere;
    public GameObject defeatBoss;
    public GameObject bonfireLit;
    public GameObject noHeals;
    public GameObject noDamageTaken;
    public AudioSource achievementSource;

    public void TriggerFirstDeath()
    {
        if (PlayerPrefs.GetInt("FirstDeath") == 0)
        {
            PlayerPrefs.SetInt("FirstDeath", 1);
            firstDeath.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerTenDeathMark()
    {
        if (PlayerPrefs.GetInt("TenDeathMark") == 0)
        {
            PlayerPrefs.SetInt("TenDeathMark", 1);
            tenDeathMark.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerIamLearning()
    {
        if (PlayerPrefs.GetInt("IamLearning") == 0)
        {
            PlayerPrefs.SetInt("IamLearning", 1);
            iAmLearning.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerAlmostThere()
    {
        if (PlayerPrefs.GetInt("AlmostThere") == 0)
        {
            PlayerPrefs.SetInt("AlmostThere", 1);
            almostThere.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerDefeatBoss()
    {
        if (PlayerPrefs.GetInt("DefeatBoss") == 0)
        {
            PlayerPrefs.SetInt("DefeatBoss", 1);
            defeatBoss.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerBonfireLit()
    {
        if (PlayerPrefs.GetInt("BonfireLit") == 0)
        {
            PlayerPrefs.SetInt("BonfireLit", 1);
            bonfireLit.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerNoHeals()
    {
        if (PlayerPrefs.GetInt("NoHeals") == 0)
        {
            PlayerPrefs.SetInt("NoHeals", 1);
            noHeals.SetActive(true);
            PlayAchievementSound();
        }
    }

    public void TriggerNoDamageTaken()
    {
        if (PlayerPrefs.GetInt("NoDamageTaken") == 0)
        {
            PlayerPrefs.SetInt("NoDamageTaken", 1);
            noDamageTaken.SetActive(true);
            PlayAchievementSound();
        }
    }

    private void PlayAchievementSound()
    {
        achievementSource.Play();
    }

}
