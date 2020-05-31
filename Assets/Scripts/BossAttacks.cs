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
    private float lastAttackTime;
    private float waitTime = 7;
    private float walkTime = 10;
    private bool moving;

    [Header("Debug")]
    public GameObject brainIcon;
    public Image bossAttackingDebug;
    public Image bossMovingDebug;
    public Text walkTimeDebug;
    public Text distanceDebug;
    public Text brainDebug;
    public Text damageDebug;

    [Header("Audio")]
    public AudioClip preFireballSound;

    private void Start()
    {
        anim = model.GetComponent<Animator>();
        lastAttackTime = Time.time;
        playerAnim = player.GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) AI = !AI;

        if (AI && !playerAnim.GetBool("Dead"))
        {
            AI_Manager(); // gerencia os movimentos do boss
        } else
        {
            DebugAttack(); // comando manual para ataque
        }

        greatSword.damageOn = anim.GetBool("Attacking"); // GreatSword causa dano apenas se o boss estiver atacando

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Straight Kick") || anim.GetCurrentAnimatorStateInfo(1).IsName("Straight Kick 0")) // controla o dano do chute
        {
            leftFootCollider.gameObject.GetComponent<DamageDealer>().damageOn = true;
        }
        else
        {
            leftFootCollider.gameObject.GetComponent<DamageDealer>().damageOn = false;
        }

        bossAttackingDebug.color = anim.GetBool("Attacking") ? Color.green : Color.red;
        damageDebug.text = greatSword.damageAmount.ToString();
        bossMovingDebug.color = moving ? Color.green : Color.red;
        brainIcon.gameObject.active = AI ? true : false; // icone que indica se a AI esta ativada ou nao

    }

    public void SpawnEarthShatter()
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

    private void FarAttack()
    {
        brainDebug.text = "Far Attack";
        anim.SetFloat("Vertical", 0);
        anim.SetFloat("Horizontal", 0);
        int rand = Random.Range(0, 5);
        switch (rand)
        {
            case 0:
                anim.SetTrigger("CastMagicSwords");
                break;
            case 1:
                anim.SetTrigger("Casting");
                break;
            case 2:
                anim.SetTrigger("Dash");
                break;
            case 3:
                anim.SetTrigger("DoubleDash");
                break;
            case 4:
                anim.SetTrigger("Spell");
                break;
            default:
                break;

        }
    }

    private void AI_Manager()
    {
        float distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player
        distanceDebug.text = distance.ToString("0.0");

        if (distance > 20) // caso o boss esteja a mais de 20 unidades, ele corre ate o player
        {
            moving = true;
            anim.SetFloat("Vertical", 1);
            anim.SetFloat("Horizontal", 0);
            brainDebug.text = "Run";
        } else if (distance > 12 && !moving) // caso ele esteja a meia distancia, comeca a caminhar
        {
            lastAttackTime = Time.time; // tempo onde comecou a caminhar
            moving = true;
            anim.SetFloat("Vertical", 0.5f);
            brainDebug.text = "Walk";
        }
        if(distance < 4) // para de caminhar caso esteja proximo ao player
        {
            anim.SetFloat("Vertical", 0);
        }
        if (anim.GetFloat("Vertical") >= 0.4f && anim.GetFloat("Vertical") < 1) // caso o boss esteja caminhando
        {
            walkTimeDebug.gameObject.SetActive(true);
            walkTimeDebug.text = (walkTime - (Time.time - lastAttackTime)).ToString("00.0");
            if ((walkTime - (Time.time - lastAttackTime)) <= 0) // caminha por no maximo 10 segundos e entao faz algum ataque a distancia
            {
                moving = false;
                FarAttack();
            }
        }
        else
        {
            walkTimeDebug.gameObject.SetActive(false);
        }


        if (moving && distance < 10) moving = false; // para de se mover caso esteja proximo ao player

        if(!anim.GetBool("Attacking") && Time.time > lastAttackTime + waitTime && !moving)
        {
            lastAttackTime = Time.time;
            waitTime = 2;

            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);

            int rand = Random.Range(0, 9);

            //print("Rand: " + rand);

            switch (rand)
            {
                case 0:
                    anim.SetTrigger("DoubleDash");
                    brainDebug.text = "Attack";
                    break;
                case 1:
                    anim.SetTrigger("Dash");
                    brainDebug.text = "Attack";
                    break;
                case 2:
                    anim.SetTrigger("SpinAttack");
                    brainDebug.text = "Attack";
                    break;
                case 3:
                    anim.SetTrigger("Combo");
                    brainDebug.text = "Attack";
                    break;
                case 4:
                    anim.SetTrigger("Casting");
                    brainDebug.text = "Casting";
                    break;
                case 5:
                    anim.SetTrigger("Combo1");
                    brainDebug.text = "Attack";
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
                    int chill = Random.Range(0, 2);
                    if(chill == 0)
                    {
                        anim.SetFloat("Horizontal", 1);
                        brainDebug.text = "Chill";
                    }
                    else
                    {
                        anim.SetFloat("Horizontal", -1);
                        brainDebug.text = "Chill";
                    }
                    break;
                default:
                    break;
            }

        }
        return;
    }

    public void TurnKickColliderOn()
    {
        leftFootCollider.enabled = true;
    }

    public void TurnKickColliderOff()
    {
        leftFootCollider.enabled = false;
    }

    public void SwordsFromSkyAttack()
    {
        StartCoroutine(DropSwordsFromSky(15));
    }

    IEnumerator DropSwordsFromSky(int counter)
    {
        float x_offset = Random.Range(-1, 1);
        float z_offset = Random.Range(-1, 1);
        GameObject earth = Instantiate(magicSwordFromSky, new Vector3(player.transform.position.x + x_offset, player.transform.position.y + 3, player.transform.position.z + z_offset), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        if(counter > 0)
            StartCoroutine(DropSwordsFromSky(counter - 1));
    }

    public void CastAura()
    {
        if(!IsBossTakingDamage()){ // confere se o boss nao esta levando dano
            Vector3 spawnPos = model.transform.position;
            spawnPos.y = 0.02f;
            GameObject aura = Instantiate(auraMagic, spawnPos, Quaternion.identity);
            aura.transform.eulerAngles = new Vector3(180, 0, 0);
        }
    }

    private void DebugAttack() // forma manual de ver os ataques
    {
        if (Input.GetKeyDown(KeyCode.G)) // ataque de longe
        {
            anim.SetTrigger("DoubleDash");
        }

        if (Input.GetKeyDown(KeyCode.H)) // ataque de perto
        {
            anim.SetTrigger("Dash");
        }

        if (Input.GetKeyDown(KeyCode.F)) // ataque de perto
        {
            anim.SetTrigger("SpinAttack");
        }

        if (Input.GetKeyDown(KeyCode.L)) // ataque de perto
        {
            anim.SetTrigger("Combo");
        }

        if (Input.GetKeyDown(KeyCode.J)) // ataque de perto
        {
            anim.SetTrigger("Combo1");
        }

        if (Input.GetKeyDown(KeyCode.M)) // ataque de perto
        {
            anim.SetTrigger("CastMagicSwords");
        }

        if (Input.GetKeyDown(KeyCode.V)) // ataque de perto
        {
            anim.SetTrigger("Casting");
        }

        if (Input.GetKeyDown(KeyCode.Z)) // ataque de perto
        {
            anim.SetTrigger("Spell");
            SoundManager.CreateAndPlay(preFireballSound, spellPosition.gameObject, 2);
        }

        if (Input.GetKeyDown(KeyCode.B)) // ataque de perto
        {
            anim.SetTrigger("AuraCast");
        }
    }

    public void FireSpell()
    {
        if (!IsBossTakingDamage())
        {
            Vector3 relativePos = player.position - spellPosition.position;
            Instantiate(spell, spellPosition.position, Quaternion.LookRotation(relativePos, Vector3.up));
        }
    }

    private bool IsBossTakingDamage() // retorna se o boss esta tomando dano, para nao executar as magias
    {
        return !anim.GetCurrentAnimatorStateInfo(2).IsName("none");
    }

}
