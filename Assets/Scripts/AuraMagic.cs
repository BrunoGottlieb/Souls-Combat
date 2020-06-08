using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraMagic : MonoBehaviour
{
    private Transform player;
    private bool isEnabled = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(DisableAura());
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(this.transform.position, player.position) < 5.5f && isEnabled) // distancia que a magia afeta o player
        {
            player.GetComponentInParent<GirlScript>().insideAuraMagic = true;
        } else
        {
            player.GetComponentInParent<GirlScript>().insideAuraMagic = false;
        }
    }

    IEnumerator DisableAura() // espera 3seg para desativar a aura
    {
        yield return new WaitForSeconds(3);
        isEnabled = false; // desativa o poder da aura
        player.GetComponentInParent<GirlScript>().insideAuraMagic = false;
        Destroy(this.gameObject);
    }
}
