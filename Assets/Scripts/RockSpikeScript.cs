using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpikeScript : MonoBehaviour
{
    public float colliderLife = 1f;
    private ParticleSystem ps;

    private void OnEnable()
    {
        StartCoroutine(DisableCollider());
    }

    IEnumerator DisableCollider()
    {
        yield return new WaitForSeconds(colliderLife);
        ps = GetComponent<ParticleSystem>();
        var coll = ps.collision;
        coll.enabled = false;
    }
}
