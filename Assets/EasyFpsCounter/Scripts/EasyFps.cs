//############################\\
//Copyrights (c) DarkSky Inc. \\
//Copyrights (c) Only Me Game \\
// https://onlymegame.com     \\
//############################\\


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//[System.Serializable]
public class EasyFps : MonoBehaviour {
    
    bool can10;
    bool can30;
    bool can60;
    bool can120;
    bool canmax;
    public UnityEvent OnFpsLessThan10;
    public UnityEvent OnFpsLessThan30;
    public UnityEvent OnFpsLessThan60;
    public UnityEvent OnFpsLessThan120;
    public UnityEvent OnFpsMoreThanMax;
    public bool ncm;
    public int maxFR;
    public float refresht = 0.5f;
    int frameCounter = 0;
    float timeCounter = 0.0f;
    float lastFramerate = 0.0f;
    bool acttxt = true;
    Text txt;                          // COMMENT THIS LINE
    //TMPro.TextMeshPro txt;          // UNCOMMENT THIS LINE
    [SerializeField]
    public float FPS
    {
        get { return lastFramerate; }
    }
    [SerializeField]
    public float RefreshTime
    {
        get { return refresht; }
        set { refresht = value; }
    }
    int mx = 60;
    public int MaxFrameRate
    {
        get { return mx; }
        set { mx = value; Application.targetFrameRate = value; }
    }

    void Start () {

        txt = transform.Find("Text").GetComponent<Text>();                         // COMMENT THIS LINE
        //txt = transform.Find("Text").GetComponent<TMPro.TextMeshPro>();         // UNCOMMENT THIS LINE

        if (ncm == true)
        {
            mx = maxFR;
            RefreshTime = refresht;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = maxFR;
        }
        EasyFpsCounter.EasyFps = this;
	}

    void Update()
    {
        if (timeCounter < refresht)
        {
            timeCounter += Time.deltaTime;
            frameCounter++;
        }
        else
        {
            lastFramerate = (float)frameCounter / timeCounter;
            int lastfrInt = (int)lastFramerate;
            if (can10 == false && lastFramerate >= 10)
            {
                can10 = true;
            }
            else if (can10 == true && lastFramerate < 10)
            {
                can10 = false;
                OnFpsLessThan10.Invoke();
            }
            if (can30 == false && lastFramerate >= 30)
            {
                can30 = true;
            }
            else if (can30 == true && lastFramerate < 30)
            {
                can30 = false;
                OnFpsLessThan30.Invoke();
            }
            if (can60 == false && lastFramerate >= 60)
            {
                can60 = true;
            }
            else if (can60 == true && lastFramerate < 60)
            {
                can60 = false;
                OnFpsLessThan60.Invoke();
            }
            if (can120 == false && lastFramerate >= 120)
            {
                can120 = true;
            }
            else if (can120 == true && lastFramerate < 120)
            {
                can120 = false;
                OnFpsLessThan120.Invoke();
            }
            if (canmax == false && lastFramerate <= MaxFrameRate)
            {
                canmax = true;
            }
            else if (canmax == true && lastFramerate > MaxFrameRate)
            {
                canmax = false;
                OnFpsMoreThanMax.Invoke();
            }
            if (acttxt == true)
            {
                if (lastFramerate <= MaxFrameRate)
                {
                    txt.text = lastfrInt.ToString();
                }
                else
                {
                    txt.text = MaxFrameRate + "+";
                }
            }
            frameCounter = 0;
            timeCounter = 0.0f;
        
        }

    }

    public void HideFps()
    {
        acttxt = false;
        txt.gameObject.SetActive(false);
    }
    public void ShowFps()
    {
        acttxt = true;
        txt.gameObject.SetActive(true);
    }

}
