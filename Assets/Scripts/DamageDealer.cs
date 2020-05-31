using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public bool damageOn; // dano esta ativo
    public float damageAmount; // quantia de dano que ira causar no player
    public AudioClip[] impactSound; // som que fara ao impactar com alguma coisa

    private float lastSoundTime = 0;

    public float GetDamage()
    {
        return damageAmount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!damageOn) return; // retorna caso nao possa causar dano

        if (other.gameObject.name == "Girl") // caso tenha colidido com o player
        {
            if (other.GetComponent<Animator>().GetBool("Intangible")) return; // nao faz dano e nem som caso o player nao possa ser acertado
            other.transform.GetComponentInParent<GirlScript>().RegisterDamage(damageAmount); // infringe o dano no player
        }

        if (SoundInterval() && impactSound.Length > 0) // caso ja deu o intervalo para poder gerar som novamente
        {
            SoundManager.CreateAndPlay(impactSound[Random.Range(0, impactSound.Length)], other.transform.root.gameObject, 2); // toca o som de impacto
            lastSoundTime = Time.time;
        }
    }

    private bool SoundInterval()
    {
        return Time.time > lastSoundTime + 0.5f;
    }

}
