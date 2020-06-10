using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockScript : MonoBehaviour
{
    public AudioClip[] rockSounds;
    private AudioSource source;
    private Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        source = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SoundVelocity() && !source.isPlaying)
        {
            source.PlayOneShot(rockSounds[Random.Range(0, rockSounds.Length)]);
        }
    }

    private bool SoundVelocity()
    {
        return (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.y) + Mathf.Abs(rb.velocity.z) > 1);
    }

    private void OnParticleCollision(GameObject other) // colisao com o ataque de impacto
    {
        // Calculate Angle Between the collision point and the player
        Vector3 dir = other.transform.position - transform.position;
        // We then get the opposite (-Vector3) and normalize it
        dir = -dir.normalized;
        // And finally we add force in the direction of dir and multiply it by force. 
        // This will push back the player
        GetComponent<Rigidbody>().AddForce(dir * 50);
    }

}
