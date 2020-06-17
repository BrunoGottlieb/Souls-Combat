using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class GirlScript : MonoBehaviour
{
    public LifeBarScript lifeBarScript;
    public Transform model;
    public Transform targetLock;
    public GameObject estusFlask;
    public GameObject healEffect;
    public GameObject bloodPrefab;
    public Transform bloodPos;
    public Transform boss;
    public Animator bossAnim;

    private float moveSpeed = 4;
    private Animator anim;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private CapsuleCollider capsuleCol;
    private Rigidbody rb;

    public AudioClip swordDamageSound;

    private float lastDamageTakenTime = 0;

    private Vector3 forwardLocked;

    [HideInInspector]
    public bool insideAuraMagic = false;
    [HideInInspector]
    public float swordCurrentDamage; // dano deste ataque da sword, setado pelo script nas animacoes

    public CameraShaker shaker;

    // Bonfire
    public Transform bonfire; // pai do bonfire
    public Text interactText; // texto dizendo para interagir com o bonfire
    private bool isBonfireLit; // controla se o bonfire esta aceso

    public AchievementManager achievementManager;
    public GameObject credits;

    void Start()
    {
        anim = model.GetComponent<Animator>();
        mainCamera = Camera.main;
        capsuleCol = model.GetComponentInChildren<CapsuleCollider>();
        rb = this.GetComponent<Rigidbody>();
        credits.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManagerScript.isBossDead) anim.SetBool("LockedCamera", false); // nao pode estar em modo de combate caso o boss esteja morto

        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (anim.GetBool("Equipped")) moveSpeed = 4.5f; // velocidade com a espada
        else moveSpeed = 6; // velocidade sem a espada

        if (anim.GetBool("Drinking")) moveSpeed = 2; // velocidade bebendo estus

        //if (anim.GetBool("Dead") || anim.GetCurrentAnimatorStateInfo(2).IsName("Sweep Fall") || anim.GetCurrentAnimatorStateInfo(2).IsName("Getting Thrown")) return; // retorna caso o jogador tenha caido ou esteja morto

        if (insideAuraMagic || GameManagerScript.gameIsPaused) // caso esteja dentro da aura magica ou o jogo esteja com o menu de pause aberto
        {
            anim.SetFloat("Speed", 0);
            anim.SetFloat("Horizontal", 0);
            anim.SetFloat("Vertical", 0);
            return;
        }

        BonfireInteraction();

        if (!anim.GetCurrentAnimatorStateInfo(2).IsName("None") || isBonfireLit) return; // retorna caso esteja tomando dano ou esteja morto

        Move();
        Rotation();
        EstusFlask();
        Attack();
        Dodge();
    }

    public void DisableEstusFlask() // metodo chamado apos o boss ser derrotado
    {
        estusFlask.SetActive(false);
    }

    private void BonfireInteraction()
    {
        if (Vector3.Distance(model.transform.position, bonfire.position) < 2.5f && bonfire.gameObject.activeSelf)
        {
            interactText.gameObject.SetActive(!isBonfireLit);
            if (Input.GetKeyDown(KeyCode.E) && !isBonfireLit)
            {
                anim.SetTrigger("LightBonfire");
                isBonfireLit = true;
                achievementManager.TriggerBonfireLit();
                credits.SetActive(true);
            }
        }
        else interactText.gameObject.SetActive(false);
    }

    private void Move()
    {
        float x = mainCamera.transform.TransformDirection(stickDirection).x;
        float z = mainCamera.transform.TransformDirection(stickDirection).z;
        if (x > 1) x = 1; // assegura que o jogador nao ira se mover mais rapido em diagonal
        if (z > 1) z = 1;

        if (anim.GetBool("CanMove"))
        {
            if(Mathf.Abs(anim.GetFloat("Speed")) > 0.15f)
                model.position += new Vector3(x * moveSpeed * Time.deltaTime, 0, z * moveSpeed * Time.deltaTime); // move o jogador para frente
            float clampValue = 1; //Input.GetKey(KeyCode.Space) ? 1 : 0.35f; // controla a velocidade de caminhar e correr
            anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, clampValue).magnitude, 0.02f, Time.deltaTime); // clamp para limitar a 1, visto que a diagonal seria de 1.4
            anim.SetFloat("Horizontal", stickDirection.x); // lockedCamera
            anim.SetFloat("Vertical", stickDirection.z); // lockedCamera
            if (anim.GetBool("Drinking") && anim.GetFloat("Speed") > 0.25f) anim.SetFloat("Speed", 0.25f); // desacelera o jogador caso ele esteja bebendo
            if (anim.GetBool("Drinking") && anim.GetFloat("Vertical") > 0.25f) anim.SetFloat("Vertical", 0.25f); // desacelera o jogador caso ele esteja bebendo
        }
        
    }

    private void DodgeController() // Dodge da locked camera
    {
        Vector3 relativeForward = mainCamera.transform.TransformDirection(Vector3.forward);
        Vector3 relativeRight = mainCamera.transform.TransformDirection(Vector3.right);
        Vector3 relativeLeft = mainCamera.transform.TransformDirection(-Vector3.right);
        Vector3 relativeBack = mainCamera.transform.TransformDirection(-Vector3.forward) * 5;

        relativeForward.y = 0;
        relativeRight.y = 0;
        relativeLeft.y = 0;
        relativeBack.y = 0;

        if (Input.GetAxis("Horizontal") > 0.1f && Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = (relativeForward + relativeRight).normalized;
        }
        else if (Input.GetAxis("Horizontal") > 0.1f && Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = (relativeBack + relativeRight).normalized;
        }
        else if (Input.GetAxis("Horizontal") < -0.1f && Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = (relativeBack + relativeLeft).normalized;
        }
        else if (Input.GetAxis("Horizontal") < -0.1f && Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = (relativeForward + relativeLeft).normalized;
        }

        else if (Input.GetAxis("Horizontal") > 0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = relativeRight;
        }
        else if (Input.GetAxis("Horizontal") < -0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = relativeLeft;
        }
        else if (Input.GetAxis("Vertical") > 0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = relativeForward;
        }
        else if (Input.GetAxis("Vertical") < -0.1f && InputManager.GetDodgeInput())
        {
            forwardLocked = relativeBack;
        }
        else if(!anim.GetBool("Dodging"))
        {
            forwardLocked = targetLock.position - model.position;
            forwardLocked.y = 0;
        }
    }

    private void Rotation()
    {
        if (anim.GetBool("Attacking")) return; // caso nao possa se mover, retorna

        if (!anim.GetBool("LockedCamera")) // camera livre
        {
            Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection) * 4f;
            rotationOffset.y = 0;
            model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * 30f);
        }
        else // camera locked
        {
            //Vector3 rotationOffset = targetLock.position - model.position;
            //rotationOffset.y = 0;

            DodgeController(); // vira instantaneamente para o lado do dodge
            model.forward += Vector3.Lerp(model.forward, forwardLocked, Time.deltaTime * 20f);
        }

    }

    private void Attack()
    {
        if (InputManager.GetPrimaryAttackInput() && anim.GetBool("CanAttack") && anim.GetBool("Equipped") && !anim.GetBool("Drinking")) // ataque primario
        {
            anim.SetTrigger("LightAttack");
        }
        if (InputManager.GetSecondaryAttackInput() && anim.GetBool("CanAttack") && anim.GetBool("Equipped") && !anim.GetBool("Drinking")) // ataque secundario
        {
            anim.SetTrigger("HeavyAttack");
        }

        if (InputManager.GetDrawSwordInput()) // botao do meio do mouse
        {
            anim.SetTrigger("Weapon");
        }

        if (InputManager.GetCameraInput() && !GameManagerScript.isBossDead) // entra e sai do modo de camera de combate
        {
            if(anim.GetBool("Equipped"))
                anim.SetBool("LockedCamera", !anim.GetBool("LockedCamera"));
        }
    }

    private void EstusFlask()
    {
        if (InputManager.GetEstusInput() && !anim.GetBool("Drinking") && !anim.GetBool("Dodging"))
        {
            anim.SetTrigger("Drink");
            //estusFlask.SetActive(true);
            StartCoroutine(DrinkEstus());
        }
    }

    IEnumerator DrinkEstus()
    {
        yield return new WaitForSeconds(1f);
        if (/*anim.GetCurrentAnimatorStateInfo(2).IsName("None") &&*/ lifeBarScript.estusFlask > 0 && !anim.GetBool("Dead")) // confere se o jogador ainda tem estus flask
        {
            lifeBarScript.UpdateLife(3);
            Instantiate(healEffect, model.position, Quaternion.identity, model.transform);
        }
        yield return new WaitForSeconds(0.5f);
        //estusFlask.SetActive(false);
        yield return new WaitForSeconds(3f);
    }

    private void Dodge()
    {
        //Vector3 diff = model.transform.eulerAngles - mainCamera.transform.eulerAngles;

        if (InputManager.GetDodgeInput() && CanDodge()) // rola caso nao esteja atacando e nem bebendo estus
        {
            anim.SetTrigger("Dodge");
        }
    }

    private bool CanMove()
    {
        return anim.GetCurrentAnimatorStateInfo(2).IsName("None");
    }

    private bool CanDodge() // Verifica se o player pode rolar
    {
        return !anim.GetBool("Attacking") && !anim.GetBool("Drinking") && !anim.GetCurrentAnimatorStateInfo(1).IsName("Sprinting Forward Roll");
    }

    private void OnParticleCollision(GameObject other)
    {
        if(other.gameObject.name.Contains("Shock") && !anim.GetBool("Intangible"))
        {
            RegisterDamage(4);
            return;
        }

        if (other.transform.root.name.Contains("Earth") && !anim.GetBool("Intangible"))
        {
            RegisterDamage(4.2f);
            return;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Contains("AuraMagic"))
            insideAuraMagic = true;
    }

    private bool DamageInterval() // garante que tenha um tempo entre um dano e outro
    {
        return (Time.time > lastDamageTakenTime + 0.25f);
    }

    public void RegisterDamage(float damageAmount)
    {
        if (damageAmount == 0 || anim.GetBool("Intangible") || !DamageInterval() || bossAnim.GetBool("Dead")) return; // retorna caso nao esteja apto a causar dano no player

        anim.SetFloat("Speed", 0);
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        lastDamageTakenTime = Time.time; // atualiza o tempo do ultimo dano tomado
        capsuleCol.isTrigger = true;
        rb.isKinematic = true;
        //anim.SetBool("Intangible", true);
        anim.SetBool("CanMove", false);
        lifeBarScript.UpdateLife(-damageAmount); // diminui a quantia de dano na vida
        DamageAnimation(damageAmount);
        shaker.ShakeCamera(0.3f);

        GameObject blood = Instantiate(bloodPrefab, bloodPos.position, Quaternion.identity);
        blood.transform.LookAt(boss.position);
        Destroy(blood, 0.2f);
    }

    private void DamageAnimation(float damageAmount)
    {
        if (damageAmount >= 4) // caso o dano seja muito forte, derruba o player
        {
            Vector3 dir = (boss.transform.position - model.transform.position).normalized; // direcao para o boss
            float dot = Vector3.Dot(dir, model.transform.forward);

            if(dot >= 0) // estava olhando para o boss, cai de costas
                anim.SetTrigger("FallDamage");
            else if (dot < 0) // estava de costas para o boss, cai de frente
                anim.SetTrigger("FallForward");
            return;
        }

        switch (Random.Range(0, 3)) // caso o dano seja pequeno sorteia uma animacao
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

}
