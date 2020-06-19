using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BossAttacks : MonoBehaviour
{
    [Header("Control")]
    public bool AI; // comanda se a inteligencia artificial estara ativada ou nao
    public bool debug; // comanda se o debug da AI aparecera na tela

    [Header("References")]
    public Transform model; // boneco do boss
    public Transform player; // garota
    public BoxCollider leftFootCollider; // pe esquerdo para o chute
    public Transform spellPosition; // mao esquerda, posicao da spell
    public Transform impactPosition; // onde sera spawnado o golpe de impacto
    private Animator playerAnim; // referencia ao animator do player, pega no start
    public DamageDealer greatSword; // script que controla o dano da GreatSword
    public CameraShaker shaker; // script na camera que treme a tela
    public GameManagerScript gameManager; // usado para pegar a booleana master

    [Header("Attacks")]
    public GameObject earthShatterPrefab;
    public GameObject magicSwordFromSky;
    public GameObject spell;
    public GameObject auraMagic;
    public GameObject screamMagic;
    public GameObject magicFarSword;
    public GameObject impactPrefab;

    private Animator anim;

    [Header("Debug")]
    public GameObject brainIcon;
    public Image bossAttackingDebug;
    public Image bossMovingDebug;
    public Text walkTimeDebug;
    public Text distanceDebug;
    public Text brainDebug;
    public Text damageDebug;
    public Text speedText;
    public Color farColor;
    public Color middleColor;
    public Color nearColor;

    [Header("AI Manager")]
    public float nearValue;
    public float farValue;
    public float chillTime;
    private string action;
    private float lastActionTime;
    private float distance;
    private float chillDirection;
    private bool phase2;
    private bool canBeginAI; // da um tempinho antes dele sair atacando pela primeira vez
    private int lastAttack; // guarda o ultimo ataque executado, para garantir que nao serao as espadas de novo

    // SlowBossDown
    private bool slowDown;
    private string actionAfterSlowDown;


    private void Start()
    {
        anim = model.GetComponent<Animator>();
        playerAnim = player.GetComponent<Animator>();

        Vector3 size = new Vector3(0.00075f, 0.0004f, 0.014f); // tamanho da GreatSword
        Vector3 center = new Vector3(0f, 0f, 0.007f);
        SetGreatSwordSize(size, center);
    }

    private void Update()
    {
        speedText.text = anim.GetFloat("Vertical").ToString("0.0");

        if (Input.GetKeyDown(KeyCode.Keypad0) && gameManager.master) AI = !AI;
        if (Input.GetKeyDown(KeyCode.Keypad1) && gameManager.master) debug = true;
        brainIcon.gameObject.SetActive(AI); // icone que indica se a AI esta ativada ou nao

        distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player

        this.transform.position = new Vector3(transform.position.x, 0, transform.position.z);

        if(debug)
            DebugUI(); // indicadores no canvas

        if (distance < 20 && !anim.GetBool("Equipped")) // pega a GreatSword quando o player chegar
        {
            anim.SetTrigger("DrawSword");
            StartCoroutine(StartAI());
        }

        if (!anim.GetBool("Equipped")) return; // caso ainda nao tenha pego a GreatSword das costas

        if (!canBeginAI) return; // caso ainda nao possa iniciar a AI

        if (AI && !playerAnim.GetBool("Dead"))
        {
            AI_Manager(); // gerencia os movimentos do boss
        } 
        else if (gameManager.master)
        {
            DebugAttack(); // comando manual para ataque
        } else
        {
            anim.SetBool("GameEnd", true); // deixa o boss no idle apos vencer
            anim.SetBool("CanRotate", false);
        }

        greatSword.damageOn = anim.GetBool("Attacking"); // GreatSword causa dano apenas se o boss estiver atacando

        phase2 = anim.GetBool("Phase2"); // coloca numa variavel para encurtar o nome

    }

    IEnumerator StartAI()
    {
        yield return new WaitForSeconds(4);
        canBeginAI = true;
    }

    private void DebugUI()
    {
        speedText.transform.parent.gameObject.SetActive(true);
        distanceDebug.transform.parent.gameObject.SetActive(true);
        brainDebug.transform.parent.gameObject.SetActive(true);
        damageDebug.text = greatSword.damageAmount.ToString();
        bossAttackingDebug.gameObject.SetActive(anim.GetBool("Attacking"));
        bossMovingDebug.gameObject.SetActive(action == "Move");
        distanceDebug.text = distance.ToString("0.0"); // mostra a distancia no debug
        if (distance <= nearValue) distanceDebug.color = nearColor;
        else if (distance >= farValue) distanceDebug.color = farColor;
        else distanceDebug.color = middleColor;
    }

    private void FarAttack()
    {
        brainDebug.text = "Far Attack";
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);

        int rand = 0;
        do
        {
            if (!anim.GetBool("Phase2")) rand = Random.Range(0, 7);
            if (anim.GetBool("Phase2")) rand = Random.Range(0, 8);
        } while (rand == lastAttack);
        lastAttack = rand;

        if (anim.GetBool("Phase2") && Random.Range(0, 2) == 0) // chance de lancar uma spell antes de um ataque de longe
        {
            anim.SetTrigger("Spell"); // Fireball
        }

        switch (rand)
        {
            case 0:
                anim.SetTrigger("CastMagicSwords"); // Magic swords from sky
                break;
            case 1:
                anim.SetTrigger("Casting"); // Earth Shatter
                break;
            case 2:
                anim.SetTrigger("Dash");
                break;
            case 3:
                anim.SetTrigger("DoubleDash");
                break;
            case 4:
                anim.SetTrigger("Spell"); // Fireball
                break;
            case 5:
                anim.SetTrigger("Scream"); 
                break;
            case 6:
                anim.SetTrigger("Fishing"); // Magic Far Sword
                break;
            case 7:
                anim.SetTrigger("SuperSpinner");
                break;
            default:
                break;
        }

        action = "Wait"; // impede que essa acao seja executada novamente
    }

    private void NearAttack()
    {
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);

        int rand = 0;
        do
        {
            if (!anim.GetBool("Phase2")) rand = Random.Range(0, 10);
            if (anim.GetBool("Phase2")) rand = Random.Range(0, 13);
        } while (rand == lastAttack);
        lastAttack = rand;

        switch (rand)
        {
            case 0:
                anim.SetTrigger("DoubleDash");
                brainDebug.text = "Double Dash";
                break;
            case 1:
                anim.SetTrigger("Dash");
                brainDebug.text = "Dash";
                break;
            case 2:
                anim.SetTrigger("SpinAttack");
                brainDebug.text = "Spin Attack";
                break;
            case 3:
                anim.SetTrigger("Combo");
                brainDebug.text = "Combo";
                break;
            case 4:
                anim.SetTrigger("Casting");
                brainDebug.text = "Casting";
                break;
            case 5:
                anim.SetTrigger("Combo1");
                brainDebug.text = "Combo1";
                break;
            case 6:
                anim.SetTrigger("Spell");
                brainDebug.text = "Spell";
                break;
            case 7:
                anim.SetTrigger("AuraCast");
                brainDebug.text = "Aura Cast";
                break;
            case 8:
                anim.SetTrigger("ForwardAttack");
                brainDebug.text = "ForwardAttack";
                break;
            case 9:
                anim.SetTrigger("Scream");
                brainDebug.text = "Scream";
                break;
            case 10:
                anim.SetTrigger("Impact");
                brainDebug.text = "Impact";
                break;
            case 11:
                anim.SetTrigger("Strong");
                brainDebug.text = "Strong";
                break;
            case 12:
                anim.SetTrigger("JumpAttack");
                brainDebug.text = "Jump Attack";
                break;
            default:
                break;
        }

        action = "Wait"; // impede que o ataque seja executado novamente

    }

    private void SlowBossDown()
    {
        if (anim.GetFloat("Vertical") <= 0.4f)
        {
            slowDown = false;
            if (actionAfterSlowDown == "CallNextMove")
            {
                action = "Wait";
                anim.SetFloat("Vertical", 0);
                anim.SetFloat("Horizontal", 0);
                StartCoroutine(WaitAfterNearMove());
            }
            else if (actionAfterSlowDown == "FarAttack")
            {
                action = "FarAttack";
            }
            else
            {
                Debug.LogError("Not supposed to be here");
            }
        }
        else
        {
            brainDebug.text = "SlowDown";
            anim.SetFloat("Vertical", Mathf.Lerp(anim.GetFloat("Vertical"), 0, 1 * Time.deltaTime));
        }
    }

    IEnumerator WaitAfterNearMove()
    {
        brainDebug.text = "WaitRandomly";
        slowDown = false;
        action = "Wait";
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        float maxWaitTime = 6;
        float possibility = 2;
        if (anim.GetBool("Phase2"))
        {
            maxWaitTime = 5.5f;
            possibility = 2;
        }
        float waitTime;
        float decision = Random.Range(0, possibility); // probabilidade de X % de que ele va esperar antes de atacar
        if (decision == 0) waitTime = Random.Range(2.5f, maxWaitTime); // tempo de espera aleatorio
        else waitTime = 0;
        yield return new WaitForSeconds(waitTime); // espera o tempo decidido antes de atacar
        action = "NearAttack";
        CallNextMove();
    }

    private void MoveToPlayer() // boss vai ate o player
    {
        brainDebug.text = "Move";

        anim.SetFloat("Horizontal", 0); // boss nao ira se mover para os lados enquanto vai na direcao do player

        float speedValue = distance / 15; // controla a velocidade com que o boss ira se mover
        if (speedValue > 1) speedValue = 1; // impede que a velocidade seja maior do que 1

        walkTimeDebug.text = (Time.time - lastActionTime).ToString("0.0");

        if (slowDown)
        {
            SlowBossDown();
            return;
        }

        if (distance < nearValue) // caso esteja proximo ao player, para de caminhar
        {
            //anim.SetFloat("Vertical", 0);
            //CallNextMove();

            actionAfterSlowDown = "CallNextMove";
            slowDown = true;
        }
        else if (Time.time - lastActionTime > chillTime) // se esta se movendo ha mais de x seg, executa um ataque
        {
            //anim.SetFloat("Vertical", 0);
            //action = "FarAttack";

            actionAfterSlowDown = "FarAttack";
            slowDown = true;
        }
        else
        {
            anim.SetFloat("Vertical", speedValue); // move-se ate o player
        }
    }

    private void WaitForPlayer() // move-se horizontalmente esperando o jogador se aproximar
    {
        brainDebug.text = "Chill";

        anim.SetFloat("Horizontal", chillDirection);
        anim.SetFloat("Vertical", 0);

        if ((distance <= nearValue && Time.time - lastActionTime > chillTime) && !phase2) // caso esteja proximo do jogador, ataca
        {
            CallNextMove();
        } else

        if((distance > farValue && Time.time - lastActionTime > chillTime) && !phase2) // caso se afastou do jogador, executa um ataque de longe
        {
            FarAttack();
        } else

        if ((Time.time - lastActionTime > chillTime) || phase2 && Time.time - lastActionTime > chillTime)
        {
            int rand = Random.Range(0, 3);

            if (rand % 2 == 0)
            {
                NearAttack();
            }
            else if (rand % 2 == 1)
            {
                FarAttack();
            }
        }

    }

    private void AI_Manager()
    {
        if (action == "Wait" || anim.GetBool("Dead") || anim.GetBool("Transposing")) return; // caso ja esteja executando alguma acao, espera

        if (action == "Move")
        {
            MoveToPlayer(); // move-se ate o player
        }

        if(action == "WaitForPlayer")
        {
            WaitForPlayer(); // move-se horizontalmente esperando o jogador se aproximar
        }

        if(action == "FarAttack")
        {
            if (!anim.GetBool("TakingDamage"))
                FarAttack(); // executa um unico ataque a longa distancia
        }

        if(action == "NearAttack")
        {
            if (!anim.GetBool("TakingDamage"))
            {
                NearAttack(); // executa um ataque de curta distancia
            }
        }
    }

    public void CallNextMove()
    {
        lastActionTime = Time.time; // atualiza o tempo onde mandou executar a ultima acao

        if (distance >= farValue && !anim.GetBool("Dead")) // caso o boss esteja longe do player
        {
            action = "Move";
        } 
        else if (distance > nearValue && distance < farValue && !anim.GetBool("Dead")) // caso esteja no meio termo
        {
            int rand = Random.Range(0, 2);
            if (rand == 0) chillDirection = -0.5f;
            if (rand == 1) chillDirection = 0.5f;
            action = "WaitForPlayer";
        }
        else if (distance <= nearValue && !anim.GetBool("Dead"))// caso esteja proximo ao jogador
        {
            action = "NearAttack";
        }
    }

    private bool IsBossTakingDamage() // retorna se o boss esta tomando dano, para nao executar as magias
    {
        return !anim.GetCurrentAnimatorStateInfo(2).IsName("none");
    }

    #region Debug

    private void DebugAttack()
    {
        anim.SetFloat("Vertical", 0);

        if (Input.GetKeyDown(KeyCode.B))
        {
            anim.SetTrigger("AuraCast");
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            anim.SetTrigger("Spell");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            anim.SetTrigger("Impact");
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("SpinAttack");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("Casting");
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetTrigger("Strong");
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            anim.SetTrigger("CastMagicSwords");
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            anim.SetTrigger("SuperSpinner");
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            anim.SetTrigger("JumpAttack");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            anim.SetTrigger("ForwardAttack");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            anim.SetTrigger("Scream");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.SetTrigger("Fishing");
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            anim.SetTrigger("Combo");
        }

    }

    #endregion

    #region Magics
    public void SpawnEarthShatter() // metodo chamado pela animacao
    {
        Vector3 bossPos = model.transform.position;
        Vector3 bossDirection = model.transform.forward;
        Quaternion bossRotation = model.transform.rotation;
        float spawnDistance = 3;

        Vector3 spawnPos = bossPos + bossDirection * spawnDistance;
        GameObject earthShatter = Instantiate(earthShatterPrefab, spawnPos, Quaternion.identity);
        earthShatter.transform.rotation = bossRotation;
        Destroy(earthShatter, 4);

        shaker.ShakeCamera(1.5f);
    }

    public void Scream()
    {
        GameObject scream = Instantiate(screamMagic, model.transform.position, Quaternion.identity);
        scream.transform.eulerAngles = new Vector3(90, 0, 0);
    }

    public void SwordsFromSkyAttack() // metodo chamado pela animacao
    {
        StartCoroutine(DropSwordsFromSky(15));
    }

    IEnumerator DropSwordsFromSky(int counter)
    {
        float x_offset = Random.Range(-1, 1); // range aleatorio
        float z_offset = Random.Range(-1, 1); // range aleatorio
        GameObject earth = Instantiate(magicSwordFromSky, new Vector3(player.transform.position.x + x_offset, player.transform.position.y + 6, player.transform.position.z + z_offset), Quaternion.identity);
        yield return new WaitForSeconds(0.25f);
        if (counter > 0) // continua spawnando espadas enquanto nao tiver spawnado todas as solicitadas
            StartCoroutine(DropSwordsFromSky(counter - 1));
    }

    public void CastAura() // metodo chamado pela animacao
    {
        if (!IsBossTakingDamage())
        { // confere se o boss nao esta levando dano
            Vector3 spawnPos = model.transform.position;
            spawnPos.y = 0.02f;
            GameObject aura = Instantiate(auraMagic, spawnPos, Quaternion.identity);
            aura.transform.eulerAngles = new Vector3(-90, 0, 0);
            aura.transform.position += new Vector3(0, 0.2f, 0);
        }
    }

    public void FireSpell() // lanca o projetil, metodo chamado pela animacao
    {
        if (!IsBossTakingDamage())
        {
            Vector3 relativePos = player.position - spellPosition.position;
            Instantiate(spell, spellPosition.position, Quaternion.LookRotation(relativePos, Vector3.up));
        }
    }

    public void Impact() // Metodo chamado pela animacao de impact attack
    {
        GameObject impactObj = Instantiate(impactPrefab, impactPosition.position, Quaternion.identity);
        Destroy(impactObj, 1.5f);
        shaker.ShakeCamera(0.5f);
    }

    public void LightGreatSwordUp()
    {
        greatSword.gameObject.GetComponent<GreatSwordScript>().EnableGreatSwordFire(); // ativa o fogo da GreatSword

        Vector3 size = new Vector3(0.00075f, 0.0004f, 0.018f);
        Vector3 center = new Vector3(0f, 0f, 0.009f);
        SetGreatSwordSize(size, center);
        greatSword.gameObject.GetComponent<GreatSwordScript>().customSize += new Vector3(0, 0, 0.012f);
    }
    private void SetGreatSwordSize(Vector3 size, Vector3 center) // altera o tamanho da hitbox da GreatSword
    {
        greatSword.gameObject.GetComponent<BoxCollider>().size = size;
        greatSword.gameObject.GetComponent<BoxCollider>().center = center;
    }

    private void MagicFarSword()
    {
        GameObject obj = Instantiate(magicFarSword, greatSword.transform.position, Quaternion.identity);
        Destroy(obj, 4.5f);
    }

    #endregion

    #region Kick

    public void TurnKickColliderOn()
    {
        leftFootCollider.enabled = true;
        leftFootCollider.GetComponent<DamageDealer>().damageOn = true;
    }

    public void TurnKickColliderOff()
    {
        leftFootCollider.enabled = false;
        leftFootCollider.GetComponent<DamageDealer>().damageOn = false;
    }

    #endregion

    private void SetNotAttackingFalse() // a cada inicio de animacao de ataque
    {
        anim.SetBool("NotAttacking", false);
    }

    private void SetNotAttackingTrue() // setado pelo None e alguns finais de animacao
    {
        anim.SetBool("NotAttacking", true);
    }

    private void SetCanRotateTrue() // Boss podera olhar para o player
    {
        anim.SetBool("CanRotate", true);
    }

    private void SetCanRotateFalse()
    {
        anim.SetBool("CanRotate", false);
    }

}
