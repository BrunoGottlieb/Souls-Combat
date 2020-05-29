using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class BossAttacks : MonoBehaviour
{
    public bool AI;
    public Transform model;
    public Transform player;
    public BoxCollider leftFootCollider;
    //public Transform greatSword;

    // Attacks
    public GameObject earthShatterPrefab;
    public GameObject magicSwordFromSky;

    private Animator anim;
    private float lastAttackTime;
    private float waitTime = 7;
    private float walkTime = 10;

    // Debug
    public GameObject brainIcon;
    public Image bossAttackingDebug;
    public Image bossMovingDebug;
    public Image walkTimeDebug;
    public Text distanceDebug;
    public Text brainDebug;

    private bool moving;

    private void Start()
    {
        anim = model.GetComponent<Animator>();
        lastAttackTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0)) AI = !AI;

        if (AI)
        {
            AI_Manager(); // gerencia os movimentos do boss
        } else
        {
            DebugAttack(); // comando manual para ataque
        }

        bossAttackingDebug.color = anim.GetBool("Attacking") ? Color.green : Color.red;
        bossMovingDebug.color = moving ? Color.green : Color.red;
        brainIcon.gameObject.active = AI ? true : false;

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
        int rand = Random.Range(0, 4);
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
            default:
                break;

        }
    }

    private void AI_Manager()
    {
        float distance = Vector3.Distance(model.transform.position, player.transform.position); // distancia do boss para o player
        distanceDebug.text = distance.ToString("0.0");

        if (distance > 20)
        {
            moving = true;
            anim.SetFloat("Vertical", 1);
            anim.SetFloat("Horizontal", 0);
            brainDebug.text = "Run";
        } else if (distance > 12 && !moving)
        {
            lastAttackTime = Time.time; // tempo onde comecou a caminhar
            moving = true;
            anim.SetFloat("Vertical", 0.5f);
            brainDebug.text = "Walk";
        }
        if (anim.GetFloat("Vertical") >= 0.4f && anim.GetFloat("Vertical") < 1)
        {
            walkTimeDebug.gameObject.SetActive(true);
            walkTimeDebug.transform.GetChild(1).GetComponent<Text>().text = (walkTime - (Time.time - lastAttackTime)).ToString("00.0");
            if ((walkTime - (Time.time - lastAttackTime)) <= 0)
            {
                moving = false;
                FarAttack();
            }
        }
        else
        {
            walkTimeDebug.gameObject.SetActive(false);
        }


        if (moving && distance < 10) moving = false;

        if(!anim.GetBool("Attacking") && Time.time > lastAttackTime + waitTime && !moving)
        {
            lastAttackTime = Time.time;
            waitTime = 2;

            anim.SetFloat("Vertical", 0);
            anim.SetFloat("Horizontal", 0);

            int rand = Random.Range(0, 8);

            print("Rand: " + rand);

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
    }
}
