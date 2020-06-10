using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    public GameObject destroyedObj; // objeto destruido que ocupara seu lugar
    public AudioClip destructionSound; // som ao ser destruido
    public GameObject sandImpactEffect; // poeira

    private void OnTriggerEnter(Collider other)
    {
        Animator otherAnim = other.transform.root.GetComponentInChildren<Animator>();
        
        if ((otherAnim != null && otherAnim.GetBool("Attacking")) && (other.gameObject.tag == "GreatSword" || other.gameObject.tag == "Sword")) // atingido por espadas
        {
            Destroy();
        }
        else if(other.gameObject.name.Contains("Magic") || other.gameObject.tag == "Magic") // atingido por magica
        {
            Destroy();
        }
    }

    private void OnCollisionEnter(Collision collision) // colisao porque eh o corpo do boss
    {
        if (collision.gameObject.name.Contains("Boss")) // atingido pelo boss se movendo
        {
            Destroy();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Destroy();
    }

    private void Destroy()
    {
        GameObject poeira = Instantiate(sandImpactEffect, this.transform.position, Quaternion.identity);
        Destroy(poeira, 2);
        Vector3 scale = this.transform.localScale;
        Instantiate(destroyedObj, transform.position, transform.rotation, transform.parent);
        destroyedObj.transform.localScale = scale;
        this.gameObject.SetActive(false);
        Destroy(this.gameObject,2);
    }

}
