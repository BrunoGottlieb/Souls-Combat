using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GirlScript : MonoBehaviour
{
    public Transform model;
    public Transform targetLock;
    public GameObject estusFlask;
    public Text lifeText;
    private float life = 10;

    private float moveSpeed = 4;
    private Animator anim;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private CapsuleCollider capsuleCol;
    private Rigidbody rb;

    public AudioClip swordDamageSound;

    void Start()
    {
        anim = model.GetComponent<Animator>();
        mainCamera = Camera.main;
        capsuleCol = model.GetComponentInChildren<CapsuleCollider>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (anim.GetBool("Equipped")) moveSpeed = 4;
        else moveSpeed = 5;

        if (anim.GetBool("Drinking")) moveSpeed = 2;

        if (anim.GetCurrentAnimatorStateInfo(2).IsName("Sweep Fall")) return; // retorna caso o jogador tenha caido

        Move();
        Rotation();
        EstusFlask();
        Attack();
        Dodge();
    }

    private void Move()
    {
        float x = mainCamera.transform.TransformDirection(stickDirection).x;
        float z = mainCamera.transform.TransformDirection(stickDirection).z;
        if (x > 1) x = 1; // assegura que o jogador nao ira se mover mais rapido em diagonal
        if (z > 1) z = 1;

        if (anim.GetBool("CanMove"))
        {
            model.position += new Vector3(x * moveSpeed * Time.deltaTime, 0, z * moveSpeed * Time.deltaTime); // move o jogador para frente
            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1).magnitude, 0.02f, Time.deltaTime); // clamp para limitar a 1, visto que a diagonal seria de 1.4
            anim.SetFloat("Horizontal", stickDirection.x);
            anim.SetFloat("Vertical", stickDirection.z);
            if (anim.GetBool("Drinking") && anim.GetFloat("Speed") > 0.25f) anim.SetFloat("Speed", 0.25f);
            if (anim.GetBool("Drinking") && anim.GetFloat("Vertical") > 0.25f) anim.SetFloat("Vertical", 0.25f);
        }
    }

    private void Rotation()
    {
        if (anim.GetBool("Attacking") || !anim.GetBool("CanMove")) return; // caso nao possa se mover, retorna

        if (!anim.GetBool("LockedCamera")) // camera livre
        {
            Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection) * 1.5f;
            rotationOffset.y = 0;
            model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 20f);
        } 
        else // camera locked
        {
            Vector3 rotationOffset = targetLock.position - model.position;
            rotationOffset.y = 0;
            model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 20f);
        }
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !anim.GetBool("Attacking") && anim.GetBool("Equipped") && !anim.GetBool("Drinking")) // ataque primario
        {
            anim.SetTrigger("LightAttack");
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !anim.GetBool("Attacking") && anim.GetBool("Equipped") && !anim.GetBool("Drinking")) // ataque secundario
        {
            anim.SetTrigger("HeavyAttack");
        }

        if (Input.GetKeyDown(KeyCode.Mouse2)) // botao do meio do mouse
        {
            anim.SetTrigger("Weapon");
        }

        if (Input.GetKeyDown(KeyCode.C)) // entra e sai do modo de camera de combate
        {
            if(anim.GetBool("Equipped"))
                anim.SetBool("LockedCamera", !anim.GetBool("LockedCamera"));
        }
    }

    private void EstusFlask()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !anim.GetBool("Drinking"))
        {
            anim.SetTrigger("Drink");
            estusFlask.SetActive(true);
            StartCoroutine(DisableEstus());
        }
    }

    IEnumerator DisableEstus()
    {
        yield return new WaitForSeconds(1f);
        UpdateLife(2);
        yield return new WaitForSeconds(0.5f);
        estusFlask.SetActive(false);
    }

    private void Dodge()
    {
        Vector3 diff = model.transform.eulerAngles - mainCamera.transform.eulerAngles;

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("Attacking") && !anim.GetBool("Drinking")) // rola caso nao esteja atacando e nem bebendo estus
        {
            model.transform.Rotate(0, 90, 0);
            anim.SetTrigger("Dodge");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        print("ACERTOU O PLAYER: " + other.transform.root.name);

        Animator otherAnim = other.transform.root.GetComponentInChildren<Animator>();
        if (other.gameObject.name.Contains("Magic") && !anim.GetBool("Intangible")) // caso sejam as espadas magicas
        {
            UpdateLife(-1.5f);
            RegisterDamage();
            RandomDamageAnimation(null);
        } else if (otherAnim != null) // caso seja um ataque do boss
            if (other.transform.root.tag == "GreatSword" && otherAnim.GetBool("Attacking") && !anim.GetBool("Intangible"))
            {
                RegisterDamage();
                RandomDamageAnimation(other.transform.root.GetComponentInChildren<Animator>());
            }
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.transform.root.name.Contains("Earth") && !anim.GetBool("Intangible"))
        {
            RegisterDamage();
            anim.SetTrigger("FallDamage");
            UpdateLife(-4);
            return;
        }
    }

    private void RegisterDamage()
    {
        CreateAndPlay(swordDamageSound, 2);
        capsuleCol.isTrigger = true;
        rb.isKinematic = true;
        anim.SetBool("Intangible", true);
        anim.SetBool("CanMove", false);
    }

    private void RandomDamageAnimation(Animator bossAnim)
    {
        if(bossAnim != null)
            if (bossAnim.GetCurrentAnimatorStateInfo(1).IsName("Straight Kick") || bossAnim.GetCurrentAnimatorStateInfo(1).IsName("Straight Kick 0"))
            {
                UpdateLife(-6f);
                anim.SetTrigger("FallDamage");
                return;
            }

        UpdateLife(-2f);
        switch (Random.Range(0, 3))
        {
            case 0:
                anim.SetTrigger("TakeDamage");
                break;
            case 1:
                anim.SetTrigger("TakeDamageLeft");
                break;
            case 2:
                anim.SetTrigger("TakeDamageRight");
                break;
        }
    }

    private void CreateAndPlay(AudioClip clip, float destructionTime, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        Destroy(audioSource, destructionTime);
    }

    private void UpdateLife(float amount)
    {
        life += amount;
        if (life > 10) life = 10;
        lifeText.text = life.ToString("0.0");
    }

}
