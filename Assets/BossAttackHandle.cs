using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackHandle : MonoBehaviour
{
    public GameObject magicSword;
    public GameObject earthShatterPrefab;
    public Transform earthShatterSpot;

    private Transform player;
    private Transform boss;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        boss = this.transform.GetChild(0).transform;
        //StartCoroutine(Wait());
    }

    public void EarthShatter()
    {
        StartCoroutine(ExecuteEarthShatter());
    }

    IEnumerator ExecuteEarthShatter()
    {
        yield return new WaitForSeconds(1.2f);
        GameObject obj = Instantiate(earthShatterPrefab, earthShatterSpot.position, Quaternion.LookRotation(player.position));
        Destroy(obj, 4);
    }

    /*IEnumerator Wait()
    {
        yield return new WaitForSeconds(5);
        StartCoroutine(DropSwords());
    }

    IEnumerator DropSwords()
    {
        yield return new WaitForSeconds(0.1f);
        float randX = Random.Range(-1.0f, 1.0f);
        float randZ = Random.Range(-1.0f, 1.0f);
        Vector3 posAbovePlayer = new Vector3(player.position.x + randX, player.position.y + 10, player.position.z + randZ);
        Instantiate(magicSword, posAbovePlayer, Quaternion.identity);
        StartCoroutine(DropSwords());
    }
    */
}
