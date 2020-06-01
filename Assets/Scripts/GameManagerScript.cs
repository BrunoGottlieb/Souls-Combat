using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerScript : MonoBehaviour
{
    public bool music;
    public AudioSource musicSource;
    public AudioSource windSource;
    private Animator bossAnim;

    private bool hasTriggeredMusic = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        bossAnim = GameObject.FindGameObjectWithTag("Boss").GetComponent<Animator>();

        if (!music)
        {
            musicSource.Stop();
            windSource.Stop();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(bossAnim.GetBool("Equipped") == true && music && !hasTriggeredMusic) // caso o boss pegou a greaSword e ainda nao havia triggado a musica
        {
            musicSource.Play();
            hasTriggeredMusic = true;
        }
    }
}
