using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public bool music; // booleana que decide se havera musica ou nao
    public bool isScenaryOn; // booleana que decide se sera o modo default ou o completo

    public AudioSource musicSource; // source que tocara a musica do jogo
    public AudioSource windSource; // som do vento antes de comecar a musica
    private bool hasTriggeredMusic = false; // booleana controla o inicio da musica quando o boss saca a GreatSword

    // Modes
    public GameObject defaultMode; // parent dos objetos do default mode
    public GameObject dustMode; // parent dos objetos do sand mode

    // Objects Material
    public Material defaultObjectsMaterial; // material branco para os objetos destrutiveis
    public Material dustObjectsMaterial; // material de areia para os objetos destrutiveis
    public GameObject[] scenaryObjects;
    [HideInInspector]
    public Material objectsMaterial; // recebe o material que esta sendo usado

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

    private void Awake()
    {
        SetEnvironmentLighting(); // seta todas as configuracoes de cena dependendo do modo escolhido
        DeliverObjectsMaterials(); // distribui o material do modo para os objetos destrutiveis do cenario
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!music)
        {
            musicSource.Stop();
            windSource.Stop();
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
            RenderSettings.ambientEquatorColor = flatColor; // modo de cor unica
            RenderSettings.skybox = dustSkybox; // ativa o skybox com o material correto
            RenderSettings.fog = true; // ativa a fog
            ambientOcclusionLayer.enabled.value = true; // ativa o ambientOcclusion
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
            ambientOcclusionLayer.enabled.value = false; // desativa o ambientOcclusion
            colorGradingLayer.contrast.value = 0f; // valor do contraste
            colorGradingLayer.mixerRedOutRedIn.value = 100f;
            colorGradingLayer.mixerBlueOutRedIn.value = 0f;
            colorGradingLayer.mixerGreenOutRedIn.value = 0f;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void BeginMusic()
    {
        if(music) musicSource.Play();
    }

}
