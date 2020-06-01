using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class BossAttacks : MonoBehaviour
{
    [Header("Control")]
    public bool AI; // comanda se a inteligencia artificial estara ativada ou nao

    [Header("References")]
    public Transform model; // boneco do boss
    public Transform player; // garota
    public BoxCollider leftFootCollider; // pe esquerdo para o chute
    public Transform spellPosition; // mao esquerda, posicao da spell
    private Animator playerAnim; // referencia ao animator do player, pega no start
    public DamageDealer greatSword; // script que controla o dano da GreatSword

    [Header("Attacks")]
    public GameObject earthShatterPrefab;
    public GameObject magicSwordFromSky;
    public GameObject spell;
    public GameObject auraMagic;

    private Animator anim;

    [Header("Debug")]
    public GameObject brainIcon;
    public Image bossAttackingDebug;
    public Image bossMovingDebug;
    public Text walkTimeDebug;
    public Text distanceDebug;
    public Text brainDebug;
    public Text damageDebug;
    public Text actionDebugText;

    [Header("Audio")]
    public AudioClip preFireballSound;

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

    private void Start()
    {
        anim = model.GetComponent<Animator>();
        playerAnim = player.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) AI = !AI;

        distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player
        distanceDebug.text = distance.ToString("0.0"); // mostra a distancia no debug

        if(distance < 15 && !anim.GetBool("Equipped")) // pega a GreatSword quando o player chegar
        {
            anim.SetTrigger("DrawSword");
            StartCoroutine(StartAI());
        }

        if (!anim.GetBool("Equipped")) return; // caso ainda nao tenha pego a GreatSword das costas

        if (!canBeginAI) return; // caso ainda nao possa iniciar a AI

        if (AI && !playerAnim.GetBool("Dead"))
        {
            AI_Manager(); // gerencia os movimentos do boss
        } else
        {
            DebugAttack(); // comando manual para ataque
        }

        greatSword.damageOn = anim.GetBool("Attacking"); // GreatSword causa dano apenas se o boss estiver atacando

        phase2 = anim.GetBool("Phase2"); // coloca numa variavel para encurtar o nome

        DebugUI(); // indicadores no canvas
    }

    IEnumerator StartAI()
    {
        yield return new WaitForSeconds(4);
        canBeginAI = true;
    }

    private void DebugUI()
    {
        bossAttackingDebug.color = anim.GetBool("Attacking") ? Color.green : Color.red;
        damageDebug.text = greatSword.damageAmount.ToString();
        bossMovingDebug.color = action == "Move" ? Color.green : Color.red;
        brainIcon.gameObject.active = AI ? true : false; // icone que indica se a AI esta ativada ou nao
        actionDebugText.text = action;
    }

    private void FarAttack()
    {
        brainDebug.text = "Far Attack";
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        int rand = Random.Range(0, 5);

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
            default:
                break;
        }

        action = "Wait"; // impede que essa acao seja executada novamente
    }

    private void NearAttack()
    {
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);

        int rand = Random.Range(0, 9);

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
                anim.SetTrigger("CastMagicSwords");
                brainDebug.text = "Magic";
                break;
            case 7:
                anim.SetTrigger("Spell");
                brainDebug.text = "Spell";
                break;
            case 8:
                anim.SetTrigger("AuraCast");
                brainDebug.text = "Aura Cast";
                break;
            default:
                break;
        }

        action = "Wait"; // impede que o ataque seja executado novamente

    }

    private void MoveToPlayer() // boss vai ate o player
    {
        brainDebug.text = "Move";

        anim.SetFloat("Horizontal", 0); // boss nao ira se mover para os lados enquanto vai na direcao do player

        float speedValue = distance / 15; // controla a velocidade com que o boss ira se mover
        if (speedValue > 1) speedValue = 1; // impede que a velocidade seja maior do que 1

        walkTimeDebug.text = (Time.time - lastActionTime).ToString("0.0");

        if (distance < nearValue) // caso esteja proximo ao player, para de caminhar
        {
            anim.SetFloat("Vertical", 0);
            CallNextMove();
        }
        else if (Time.time - lastActionTime > chillTime) // se esta se movendo ha mais de x seg, executa um ataque
        {
            anim.SetFloat("Vertical", 0);
            action = "FarAttack";
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

        if ((distance <= nearValue && Time.time - lastActionTime > chillTime/2) && !phase2) // caso esteja proximo do jogador, ataca
        {
            CallNextMove();
        } else

        if((distance > farValue && Time.time - lastActionTime > chillTime/2) && !phase2) // caso se afastou do jogador, executa um ataque de longe
        {
            FarAttack();
        } else

        if ((Time.time - lastActionTime > chillTime) || phase2)
        {
            if (chillDirection < 0)
            {
                FarAttack();
            } else
            {
                NearAttack();
            }
        }

    }

    private void AI_Manager()
    {
        if (action == "Wait") return; // caso ja esteja executando alguma acao, espera

        if(action == "Move")
        {
            MoveToPlayer(); // move-se ate o player
        }

        if(action == "WaitForPlayer")
        {
            WaitForPlayer(); // move-se horizontalmente esperando o jogador se aproximar
        }

        if(action == "FarAttack")
        {
            FarAttack(); // executa um unico ataque a distancia
        }

        if(action == "NearAttack")
        {
            NearAttack();
        }
    }

    public void CallNextMove()
    {
        lastActionTime = Time.time; // atualiza o tempo onde mandou executar a ultima acao

        if (distance >= farValue) // caso o boss esteja longe do player
        {
            action = "Move";
        } 
        else if (distance > nearValue && distance < farValue) // caso esteja no meio termo
        {
            int rand = Random.Range(0, 2);
            if (rand == 0) chillDirection = -0.5f;
            if (rand == 1) chillDirection = 0.5f;
            action = "WaitForPlayer";
        }
        else if (distance <= nearValue)// caso esteja proximo ao jogador
        {
            action = "NearAttack";
        }
    }

    private bool IsBossTakingDamage() // retorna se o boss esta tomando dano, para nao executar as magias
    {
        return !anim.GetCurrentAnimatorStateInfo(2).IsName("none");
    }

    private void DrawGreatSword()
    {

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

        if (Input.GetKeyDown(KeyCode.F))
        {
            anim.SetTrigger("SpinAttack");
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("Casting");
        }
    }

    #endregion

    #region Magias
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
    }

    public void SwordsFromSkyAttack() // metodo chamado pela animacao
    {
        StartCoroutine(DropSwordsFromSky(15));
    }

    IEnumerator DropSwordsFromSky(int counter)
    {
        float x_offset = Random.Range(-1, 1); // range aleatorio
        float z_offset = Random.Range(-1, 1); // range aleatorio
        GameObject earth = Instantiate(magicSwordFromSky, new Vector3(player.transform.position.x + x_offset, player.transform.position.y + 3, player.transform.position.z + z_offset), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
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
            aura.transform.eulerAngles = new Vector3(180, 0, 0);
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

}
