using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScript : MonoBehaviour
{
    public string playerPrefName;
    private Animator anim;

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        if(anim == null) anim = this.GetComponent<Animator>();
        if (PlayerPrefs.GetInt(playerPrefName) == 1) anim.enabled = true;
        else anim.enabled = false;
    }
}
