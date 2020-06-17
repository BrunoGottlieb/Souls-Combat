using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;

[DefaultExecutionOrder(0)]
public class GameManagerScript : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    // Control
    public bool master; // permite comandar o jogo como administrador
    public bool resetCacheOnStart;
    [HideInInspector]
    public bool music; // booleana que decide se havera musica ou nao
    [HideInInspector]
    public bool isScenaryOn; // booleana que decide se sera o modo default ou o completo
    private bool isStonesOnGroundOn; // booleana que decide se havera pedras no chao do cenario da dust
    private bool isHighQualityOn; // booleana que decide se o jogo estara no ultra
    private bool isVSyncOn; // booleana que decide se o VSync estara ligado ou nao
    private bool isObjectsOn; // booleana que decide se terao objetos no cenario
    private bool showFPS; // booleana que decide se o contador de FPS sera exibido ou nao
    public static bool gameHasStarted; // controla se o jogo ja iniciou, para que o personagem nao possa se mover antes da hora
    public static bool isBossDead; // indica se o boss esta morto
    [HideInInspector]
    public bool isAutoRestartOn; // booleana que decide se o jogo se reinicia sozinho apos o jogador morrer

    public AudioSource musicSource; // source que tocara a musica do jogo
    public AudioSource windSource; // som do vento antes de comecar a musica

    // Modes
    public GameObject defaultMode; // parent dos objetos do default mode
    public GameObject dustMode; // parent dos objetos do sand mode

    // Objects Material
    public Material defaultObjectsMaterial; // material branco para os objetos destrutiveis
    public Material dustObjectsMaterial; // material de areia para os objetos destrutiveis
    public GameObject[] scenaryObjects;
    [HideInInspector]
    public Material objectsMaterial; // recebe o material que esta sendo usado
    public GameObject stonesOnGround; // pedras que ficam no chao da dust mode
    public GameObject raycaster; // desativa junto com as pedras no chao
    public GameObject destructibleObjects; // objetos do cenario
    public GameObject FPSCounter; // contador de FPS
    public GameObject dustStorm; // tempestade de areia

    // Lightining Settings
    public Color skyColor; // gradiente do modo default
    public Color equatorColor; // gradiente do modo default
    public Color groundColor; // gradiente do modo default
    public Color flatColor; // cor unica que ilumina a cena do modo dust
    public PostProcessVolume volume; // post-processing
    private AmbientOcclusion ambientOcclusionLayer = null; // recebe o ambiente occlusion do post-processing
    private ColorGrading colorGradingLayer = null; // recebe o colorGrading do post-processing
    private MotionBlur motionBlur = null;

    // Skybox materials
    public Material dustSkybox; // skybox laranja da dust
    private Material defaultSkybox = null; // modo default nao possui skybox

    public CanvasGroup transitionFade;
    private bool restarting;

    // pause screen
    public GameObject pauseScreen;
    public static bool gameIsPaused = false;

    // Espadas
    public SwordScript swordScript;
    public GreatSwordScript greatSwordScript;

    public bool playerIsDead;
    private float keyInterval;

    private void Awake()
    {
        if(resetCacheOnStart) PlayerPrefs.DeleteAll();

        HideCursor(true);

        CheckForChanges();

        SetEnvironmentLighting(); // seta todas as configuracoes de cena dependendo do modo escolhido
    }

    void Start()
    {
        StartCoroutine(TransitionFadeOut());
        isBossDead = false;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.Keypad4) && Input.GetKey(KeyCode.Keypad2) && Time.time - keyInterval > 0.5f)
        {
            keyInterval = Time.time;
            master = !master;
            print("Master: " + master);
        }

        if(Input.GetKey(KeyCode.Q) && Input.GetKey(KeyCode.Backspace) && Time.time - keyInterval > 0.5f)
        {
            keyInterval = Time.time;
            PlayerPrefs.DeleteAll();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter) && master && Time.timeScale > 0.5f) Time.timeScale = 0.1f;
        else if (Input.GetKeyDown(KeyCode.KeypadEnter) && master && Time.timeScale < 1) Time.timeScale = 1f;

        if (InputManager.GetRestartInput() && !restarting && !gameIsPaused && (playerIsDead || master))
        {
            Restart();
        }

        if (InputManager.GetPauseInput() && !restarting)
        {
            PauseManager();
        }

        if (gameIsPaused && PlayerPrefs.GetInt("IsMusicOn") == 1)
        {
            musicSource.volume = 0.3f;
        } 
        else if (PlayerPrefs.GetInt("IsMusicOn") == 1 && !isBossDead)
        {
            musicSource.volume = 1f;
        }

    }

    public void CheckForChanges()
    {
        if (!PlayerPrefs.HasKey("IsObjectsOn")) PlayerPrefs.SetInt("IsObjectsOn", 1);
        if (!PlayerPrefs.HasKey("IsFPSOn")) PlayerPrefs.SetInt("IsFPSOn", 0);
        if (!PlayerPrefs.HasKey("IsMusicOn")) PlayerPrefs.SetInt("IsMusicOn", 1);
        if (!PlayerPrefs.HasKey("AutoRestart")) PlayerPrefs.SetInt("AutoRestart", 0);
        if (!PlayerPrefs.HasKey("IsVSyncOn")) PlayerPrefs.SetInt("IsVSyncOn", 0);
        if (!PlayerPrefs.HasKey("IsScenaryOn")) PlayerPrefs.SetInt("IsScenaryOn", 1);
        if (!PlayerPrefs.HasKey("StonesOnGround")) PlayerPrefs.SetInt("StonesOnGround", 1);
        if (!PlayerPrefs.HasKey("IsHighQualityOn")) PlayerPrefs.SetInt("IsHighQualityOn", 1);
        if (!PlayerPrefs.HasKey("DustStorm")) PlayerPrefs.SetInt("DustStorm", 1);
        if (!PlayerPrefs.HasKey("BetterColliders")) PlayerPrefs.SetInt("BetterColliders", 1);
        if (!PlayerPrefs.HasKey("MotionBlur")) PlayerPrefs.SetInt("MotionBlur", 1);
        if (!PlayerPrefs.HasKey("PS4Input")) PlayerPrefs.SetInt("PS4Input", 0);
        /*
        print("IsObjectsOn: " + PlayerPrefs.GetInt("IsObjectsOn"));
        print("IsFPSOn: " + PlayerPrefs.GetInt("IsFPSOn"));
        print("IsMusicOn: " + PlayerPrefs.GetInt("IsMusicOn"));
        print("AutoRestart: " + PlayerPrefs.GetInt("AutoRestart"));
        print("IsVSyncOn: " + PlayerPrefs.GetInt("IsVSyncOn"));
        print("IsScenaryOn: " + PlayerPrefs.GetInt("IsScenaryOn"));
        print("StonesOnGround: " + PlayerPrefs.GetInt("StonesOnGround"));
        print("IsHighQualityOn: " + PlayerPrefs.GetInt("IsHighQualityOn"));
        print("DustStorm: " + PlayerPrefs.GetInt("DustStorm"));
        print("BetterColliders: " + PlayerPrefs.GetInt("BetterColliders"));
        */
        isObjectsOn = PlayerPrefs.GetInt("IsObjectsOn") == 1 ? true : false;
        showFPS = PlayerPrefs.GetInt("IsFPSOn") == 1 ? true : false;
        music = PlayerPrefs.GetInt("IsMusicOn") == 1 ? true : false;
        isAutoRestartOn = PlayerPrefs.GetInt("AutoRestart") == 1 ? true : false;
        isVSyncOn = PlayerPrefs.GetInt("IsVSyncOn") == 1 ? true : false;
        isScenaryOn = PlayerPrefs.GetInt("IsScenaryOn") == 1 ? true : false;
        isStonesOnGroundOn = PlayerPrefs.GetInt("StonesOnGround") == 1? true : false;
        isHighQualityOn = PlayerPrefs.GetInt("IsHighQualityOn") == 1 ? true : false;

        CheckDesctructibleObjetcsState();
        CheckShowFPSState();
        CheckMusicState();
        CheckAutoRestartState();
        CheckVSyncState();
        CheckScenaryState();
        CheckStonesOnGroundState();
        CheckHighQualityState();
        CheckSwordColliders();
        CheckDustStorm();
        CheckPS4Input();
    }

    public void Restart()
    {
        restarting = true;
        StartCoroutine(TransitionFadeIn());
    }

    IEnumerator TransitionFadeOut()
    {
        transitionFade.gameObject.SetActive(true);
        transitionFade.alpha = 1;
        while (transitionFade.alpha > 0) // inicia o fade da transicao entre cenas
        {
            transitionFade.alpha -= 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        transitionFade.gameObject.SetActive(false);
    }

    public static void HideCursor(bool b)
    {
        if (b) // hide
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else // visible
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            int xPos = Screen.width/2, yPos = Screen.height/3;
            SetCursorPos(xPos, yPos);//Call this when you want to set the mouse position
        }
    }

    private void SetEnvironmentLighting()
    {
        volume.profile.TryGetSettings(out ambientOcclusionLayer);
        volume.profile.TryGetSettings(out colorGradingLayer);

        if (isScenaryOn) // DUST MODE
        {
            dustMode.SetActive(true); // ativa os objetos exclusivos do dustMode
            defaultMode.SetActive(false); // desativa os objetos exclusivos do default mode
            objectsMaterial = dustObjectsMaterial; // seta o material dos objetos destrutiveis como os de areia
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = flatColor; // modo de cor unica
            RenderSettings.skybox = dustSkybox; // ativa o skybox com o material correto
            RenderSettings.fog = true; // ativa a fog
            //ambientOcclusionLayer.enabled.value = true; // ativa o ambientOcclusion
            ambientOcclusionLayer.intensity.value = 1.6f; // ativa o ambientOcclusion
            colorGradingLayer.contrast.value = 10f; // valor do contraste
            colorGradingLayer.mixerRedOutRedIn.value = 100f;
            colorGradingLayer.mixerBlueOutRedIn.value = -150f;
            colorGradingLayer.mixerGreenOutRedIn.value = 40f;
        }
        else // DEFAULT MODE
        {
            RenderSettings.ambientMode = AmbientMode.Trilight; // seta o modo de luz da cena como gradiente
            RenderSettings.ambientSkyColor = skyColor;
            RenderSettings.ambientEquatorColor = equatorColor;
            RenderSettings.ambientGroundColor = groundColor;
            objectsMaterial = defaultObjectsMaterial; // seta o material dos objetos destrutiveis como os defaults brancos
            defaultMode.SetActive(true); // ativa os objetos exclusivos do default mode
            dustMode.SetActive(false); // desativa os objetos exclusivos do dustMode
            RenderSettings.skybox = defaultSkybox; // desativa o skybox
            RenderSettings.fog = false; // desativa a fog
            //ambientOcclusionLayer.enabled.value = false; // desativa o ambientOcclusion
            ambientOcclusionLayer.intensity.value = 0f; // desativa o ambientOcclusion
            colorGradingLayer.contrast.value = 0f; // valor do contraste
            colorGradingLayer.mixerRedOutRedIn.value = 100f;
            colorGradingLayer.mixerBlueOutRedIn.value = 0f;
            colorGradingLayer.mixerGreenOutRedIn.value = 0f;
        }

    }

    private void PauseManager()
    {
        if (!pauseScreen.activeSelf) // Ativa a tela de pause
        {
            HideCursor(true);
            pauseScreen.SetActive(true);
        }

        //Time.timeScale = Time.timeScale == 1 ? Time.timeScale = 0 : Time.timeScale = 1;
    }

    IEnumerator TransitionFadeIn()
    {
        transitionFade.gameObject.SetActive(true);
        transitionFade.alpha = 0;
        while (transitionFade.alpha < 1) // inicia o fade da transicao entre cenas
        {
            transitionFade.alpha += 0.05f;
            yield return new WaitForSeconds(0.1f);
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BeginMusic()
    {
        if(music) musicSource.Play();
    }

    // ----------------------------------------------------- CONFIGURATION -----------------------------------------------------

    private void CheckMotionBlur()
    {
        if (PlayerPrefs.GetInt("MotionBlur") == 1)
        {
            if (motionBlur == null) volume.profile.TryGetSettings(out motionBlur);
            motionBlur.active = true;
        }
        else
        {
            if (motionBlur == null) volume.profile.TryGetSettings(out motionBlur);
            motionBlur.active = false;
        }
    }

    public void CheckMusicState() // chamado pelo botao na tela de configuracoes
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("IsMusicOn") == 1 && !musicSource.isPlaying && gameHasStarted)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void CheckScenaryState()
    {
        if (!Application.isPlaying) return;
        SetEnvironmentLighting();
    }

    public void CheckStonesOnGroundState()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("StonesOnGround") == 1)
        {
            stonesOnGround.SetActive(true);
            raycaster.SetActive(true);
            isStonesOnGroundOn = true;
        }
        else
        {
            stonesOnGround.SetActive(false);
            raycaster.SetActive(false);
            isStonesOnGroundOn = false;
        }
    }

    public void CheckHighQualityState()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("IsHighQualityOn") == 1)
        {
            QualitySettings.SetQualityLevel(5);
            isHighQualityOn = true;
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
            isHighQualityOn = false;
        }
    }

    public void CheckVSyncState()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("IsVSyncOn") == 1)
        {
            QualitySettings.vSyncCount = 1;
            isVSyncOn = true;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            isVSyncOn = false;
        }
    }

    public void CheckShowFPSState()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("IsFPSOn") == 1)
        {
            FPSCounter.SetActive(true);
            showFPS = true;
        }
        else
        {
            FPSCounter.SetActive(false);
            showFPS = false;
        }
    }

    public void CheckDesctructibleObjetcsState()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("IsObjectsOn") == 1)
        {
            destructibleObjects.SetActive(true);
            isObjectsOn = true;
        }
        else
        {
            isObjectsOn = false;
            destructibleObjects.SetActive(false);
        }
    }

    public void CheckAutoRestartState()
    {
        if (!Application.isPlaying) return;
        if (isAutoRestartOn)
        {
            PlayerPrefs.SetInt("AutoRestart", 1);
        }
        else
        {
            PlayerPrefs.SetInt("AutoRestart", 0);
        }
    }

    public void CheckDustStorm()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("DustStorm") == 1)
        {
            dustStorm.SetActive(true);
        } else
        {
            dustStorm.SetActive(false);
        }
    }

    public void CheckPS4Input()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("PS4Input") == 1)
        {
            InputManager.PS4Inputs = true;
        }
        else
        {
            InputManager.PS4Inputs = false;
        }
    }

    public void CheckSwordColliders()
    {
        if (!Application.isPlaying) return;
        if (PlayerPrefs.GetInt("BetterColliders") == 1)
        {
            swordScript.betterColliders = true;
            greatSwordScript.betterColliders = true;
        } else
        {
            swordScript.betterColliders = false;
            greatSwordScript.betterColliders = false;
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        int totalTime = (int)(PlayerPrefs.GetInt("TotalTime") + Time.time); // Atualiza o tempo total
        PlayerPrefs.SetInt("TotalTime", totalTime); // armazena o valor
    }

}
