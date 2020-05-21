using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    public Transform Target;
    public float firingAngle = 60.0f;
    public float gravity = 9.8f;

    public Transform Projectile;
    private Transform myTransform;
    private Animator anim;

    void Awake()
    {
        myTransform = transform;
        anim = this.GetComponent<Animator>();
    }

    void Start()
    {
        //StartCoroutine(SimulateProjectile());
    }

    public void Saltar()
    {
        StartCoroutine(SimulateProjectile());
    }


    IEnumerator SimulateProjectile()
    {
        // Short delay added before Projectile is thrown
        yield return new WaitForSeconds(0.45f);

        // Move projectile to the position of throwing object + add some offset if needed.
        Projectile.position = myTransform.position + new Vector3(0, 0.0f, 0);

        // Calculate distance to target
        float target_Distance = Vector3.Distance(Projectile.position, Target.position);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        // Rotate projectile to face the target.
        //Projectile.rotation = Quaternion.LookRotation(Target.position - Projectile.position);

        float elapse_time = 0;

        while (elapse_time < flightDuration-0.2f)
        {
            Projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            if(elapse_time / flightDuration >= 0.5f)
            {
                anim.Play("Paladin Jump End");
            }

            yield return null;
        }
    }
}