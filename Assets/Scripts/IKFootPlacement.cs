using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class IKFootPlacement : MonoBehaviour
{
    private Animator anim;
    [Range(0f,1f)]
    public float distanceToGround;
    public LayerMask layerMask;

    private Rigidbody rb;
    private CapsuleCollider capsuleCol;

    public GameObject backWeapon;
    public GameObject handWeapon;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        capsuleCol = this.GetComponent<CapsuleCollider>();
        rb = GetComponentInParent<Rigidbody>();
        handWeapon.SetActive(false); // espada da mao comeca desativada
    }

    public void TakeWeapon()
    {
        if(!anim.GetBool("Equipped"))
        {
            backWeapon.SetActive(false);
            handWeapon.SetActive(true);
            anim.SetBool("Equipped", true);
        }
        else
        {
            backWeapon.SetActive(true);
            handWeapon.SetActive(false);
            anim.SetBool("Equipped", false);
            anim.SetBool("LockedCamera", false);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);

        RaycastHit hit;
        Ray ray = new Ray(anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        {
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation);
            }
        }

        ray = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask))
        {
            if (hit.transform.tag == "Walkable")
            {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hit.normal);
                anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation);
            }

        }
    }

    public void SetIntangibleOn()
    {
        //capsuleCol.isTrigger = true;
        //rb.isKinematic = true;
        anim.SetBool("Intangible", true);
    }

    public void RestoreRigidbodyAndCollider()
    {
        capsuleCol.isTrigger = false;
        rb.isKinematic = false;
        //anim.SetBool("CanMove", true);
        anim.SetBool("Intangible", false);
    }

    public void SetAttackingTrue()
    {
        anim.SetBool("Attacking", true);
    }

    public void SetAttackingFalse()
    {
        anim.SetBool("Attacking", false);
    }

    public void SetCanAttackTrue()
    {
        anim.SetBool("CanAttack", true);
    }

}
