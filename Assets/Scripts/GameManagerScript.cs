using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;

public class GameManagerScript : MonoBehaviour
{
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);

    // Control
    public bool music; // booleana que decide se havera musica ou nao
    public bool isScenaryOn; // booleana que decide se sera o modo default ou o completo
    private bool isStonesOnGroundOn; // booleana que decide se havera pedras no chao do cenario da dust
    private bool isHighQualityOn; // booleana que decide se o jogo estara no ultra
    private bool isVSyncOn; // booleana que decide se o VSync estara ligado ou nao
    private bool isObjectsOn; // booleana que decide se terao objetos no cenario
    private bool showFPS; // booleana que decide se o contador de FPS sera exibido ou nao
    public static bool gameHasStarted; // controla se o jogo ja iniciou, para que o personagem nao possa se mover antes da hora
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
    public GameObject destructibleObjects; // objetos do cenario
    public GameObject FPSCounter; // contador de FPS

    // Lightining Settings
    public Color skyColor; // gradiente do modo default
    public Color equatorColor; // gradiente do modo default
    public Color groundColor; // gradiente do modo default
    public Color flatColor; // cor unica que ilumina a cena do modo dust
    public PostProcessVolume volume; // post-processing
    private AmbientOcclusion ambientOcclusionLayer = null; // recebe o ambiente occlusion do post-processing
    private ColorGrading colorGradingLayer = null; // recebe o colorGrading do post-processing

    // Skybox materials
    public Material dustSkybox; // skybox laranja da dust
    private Material defaultSkybox = null; // modo default nao possui skybox

    public CanvasGroup transitionFade;
    private bool restarting;

    // pause screen
    public GameObject pauseScreen;
    public static bool gameIsPaused = false;

    private void Awake()
    {
        SetEnvironmentLighting(); // seta todas as configuracoes de cena dependendo do modo escolhido
        DeliverObjectsMaterials(); // distribui o material do modo para os objetos destrutiveis do cenario
    }

    void Start()
    {
        StartCoroutine(TransitionFadeOut());

        SetGameStart();

    }

    void Update()
    {
        if (InputManager.GetRestartInput() && !restarting && !gameIsPaused)
        {
            Restart();
        }

        if (InputManager.GetPauseInput() && !restarting)
        {
            PauseManager();
        }

    }

    private void SetGameStart()
    {
        HideCursor(true);

        if (!PlayerPrefs.HasKey("IsObjectsOn")) PlayerPrefs.SetInt("IsObjectsOn", 1);
        if (!PlayerPrefs.HasKey("IsFPSOn")) PlayerPrefs.SetInt("IsFPSOn", 0);
        if (!PlayerPrefs.HasKey("IsMusicOn")) PlayerPrefs.SetInt("IsMusicOn", 1);
        if (!PlayerPrefs.HasKey("AutoRestart")) PlayerPrefs.SetInt("AutoRestart", 0);
        if (!PlayerPrefs.HasKey("IsVSyncOn")) PlayerPrefs.SetInt("IsVSyncOn", 0);
        if (!PlayerPrefs.HasKey("IsScenaryOn")) PlayerPrefs.SetInt("IsScenaryOn", 1);
        if (!PlayerPrefs.HasKey("StonesOnGround")) PlayerPrefs.SetInt("StonesOnGround", 1);
        if (!PlayerPrefs.HasKey("IsHighQualityOn")) PlayerPrefs.SetInt("IsHighQualityOn", 1);


        isObjectsOn = PlayerPrefs.GetInt("IsObjectsOn") == 1 ? true : false;
        showFPS = PlayerPrefs.GetInt("IsFPSOn") == 1 ? true : false;
        music = PlayerPrefs.GetInt("IsMusicOn") == 1 ? true : false;
        isAutoRestartOn = PlayerPrefs.GetInt("AutoRestart") == 1 ? true : false;
        isVSyncOn = PlayerPrefs.GetInt("IsVSyncOn") == 1 ? true : false;
        isScenaryOn = PlayerPrefs.GetInt("IsScenaryOn") == 1 ? true : false;
        isStonesOnGroundOn = PlayerPrefs.GetInt("StonesOnGround") == 1? true : false;
        isHighQualityOn = PlayerPrefs.GetInt("IsHighQualityOn") == 1 ? true : false;

        ApplyDesctructibleObjetcsState();
        ApplyFPSState();
        ApplyMusicState();
        ApplyVSyncState();
        SetEnvironmentLighting();
        ApplyStonesOnGroundState();
        ApplyHighQualityState();
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

    private void DeliverObjectsMaterials()
    {
        foreach(GameObject objeto in scenaryObjects)
        {
            Renderer r = objeto.GetComponent<Renderer>();
            if(r != null)
            {
                r.material = objectsMaterial;
            } else
            {
                objeto.GetComponentInChildren<Renderer>().material = objectsMaterial;
            }
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
            PlayerPrefs.SetInt("IsScenaryOn", 1);
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
            PlayerPrefs.SetInt("IsScenaryOn", 0);
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

    // Configuration

    public void ChangeMusicState() // chamado pelo botao na tela de configuracoes
    {
        music = !music;
        ApplyMusicState();
    }

    public void ChangeScenaryState()
    {
        isScenaryOn = !isScenaryOn;
        SetEnvironmentLighting();
    }

    public void ChangeStonesOnGroundState()
    {
        isStonesOnGroundOn = !isStonesOnGroundOn;
        ApplyStonesOnGroundState();
    }

    public void ChangeHighQualityState()
    {
        isHighQualityOn = !isHighQualityOn;
        ApplyHighQualityState();
    }

    public void ChangeVSyncState()
    {
        isVSyncOn = !isVSyncOn;
        ApplyVSyncState();
    }

    public void ChangeShowFPSState()
    {
        showFPS = !showFPS;
        ApplyFPSState();
    }

    public void ChangeDesctructibleObjetcsState()
    {
        isObjectsOn = !isObjectsOn;
        ApplyDesctructibleObjetcsState();
    }

    public void ChangeAutoRestartState()
    {
        isAutoRestartOn = !isAutoRestartOn;
        if (isAutoRestartOn)
        {
            PlayerPrefs.SetInt("AutoRestart", 1);
        } 
        else
        {
            PlayerPrefs.SetInt("AutoRestart", 0);
        }
    }

    private void ApplyDesctructibleObjetcsState()
    {
        if (isObjectsOn)
        {
            destructibleObjects.SetActive(true);
            PlayerPrefs.SetInt("IsObjectsOn", 1);
        }
        else
        {
            destructibleObjects.SetActive(false);
            PlayerPrefs.SetInt("IsObjectsOn", 0);
        }
    }

    private void ApplyMusicState()
    {
        if (music)
        {
            musicSource.Play();
            PlayerPrefs.GetInt("IsMusicOn", 1);
        }
        else
        {
            musicSource.Stop();
            PlayerPrefs.GetInt("IsMusicOn", 0);
        }
    }

    private void ApplyFPSState()
    {
        if (showFPS)
        {
            FPSCounter.SetActive(true);
            PlayerPrefs.SetInt("IsFPSOn", 1);
        }
        else
        {
            FPSCounter.SetActive(false);
            PlayerPrefs.SetInt("IsFPSOn", 0);
        }
    }

    private void ApplyVSyncState()
    {
        if (isVSyncOn)
        {
            QualitySettings.vSyncCount = 1;
            PlayerPrefs.SetInt("IsVSyncOn", 1);
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            PlayerPrefs.SetInt("IsVSyncOn", 0);
        }
    }

    private void ApplyStonesOnGroundState()
    {
        if (isStonesOnGroundOn)
        {
            stonesOnGround.SetActive(true);
            PlayerPrefs.SetInt("StonesOnGround", 1);
        }
        else
        {
            stonesOnGround.SetActive(false);
            PlayerPrefs.SetInt("StonesOnGround", 0);
        }
    }

    private void ApplyHighQualityState()
    {
        if (isHighQualityOn)
        {
            QualitySettings.SetQualityLevel(5);
            PlayerPrefs.SetInt("IsHighQualityOn", 1);
        }
        else
        {
            QualitySettings.SetQualityLevel(0);
            PlayerPrefs.SetInt("IsHighQualityOn", 0);
        }
    }

}
