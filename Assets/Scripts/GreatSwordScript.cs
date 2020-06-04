using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatSwordScript : MonoBehaviour
{
    public Animator bossAnim;
    public GameObject dustExplosionPrefab;
    private float lastTime;
    public ParticleSystem ps;

    private void Update()
    {
        if (bossAnim.GetBool("Phase2") && bossAnim.GetBool("Attacking"))
        {
            this.transform.GetChild(1).gameObject.SetActive(true);
            /*var em = ps.emission;
            em.enabled = true;*/
            lastTime = Time.time;
        }
        else if (Time.time + 1 > lastTime)
        {
            this.transform.GetChild(1).gameObject.SetActive(false);
            /*var em = ps.emission;
            em.enabled = false;*/
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((bossAnim.GetBool("Attacking") || other.gameObject.layer == 9) && (other.gameObject.layer == 9 || other.gameObject.layer == 13) && Time.time > lastTime + 0.1f) // Ground ou Scenary
        {
            if(dustExplosionPrefab != null)
            {
                GameObject dustEx = Instantiate(dustExplosionPrefab, this.transform.position, Quaternion.identity);
                Destroy(dustEx, 1);
            }
            lastTime = Time.time;
        }
    }

}
