using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    [Range(20f, 80f)]
    public float rotationSpeed;
    public float turnSpeed = 1f;

    public Transform model;
    public Transform targetLock; 

    private Animator anim;
    private Vector3 stickDirection;
    private Camera mainCamera;

    private bool isWeaponEquipped = false;
    private bool isTargetLocked = false;
    private bool canRotate = true; // usado para impedir o boneco de rotacionar enquanto ataca

    void Start()
    {
        mainCamera = Camera.main;
        anim = model.GetComponent<Animator>();
    }

    void Update()
    {
        stickDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        HandleInputData();
        if (isTargetLocked && canRotate)
            HandleTargetLockedLocomotionRotation();
        else if(canRotate)
            HandleStandardLocomotionRotation();

        if (canRotate)
            model.transform.position += new Vector3(mainCamera.transform.TransformDirection(stickDirection).x * 5 * Time.deltaTime, 0, mainCamera.transform.TransformDirection(stickDirection).z * 5 * Time.deltaTime);
    }

    private void HandleStandardLocomotionRotation()
    {
        if (!canRotate) return;
        Vector3 rotationOffset = mainCamera.transform.TransformDirection(stickDirection);
        rotationOffset.y = 0;
        model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * rotationSpeed);
    }

    private void HandleTargetLockedLocomotionRotation() // neste caso a referencia de rotacao eh o target
    {
        Vector3 rotationOffset = targetLock.transform.position - model.position;
        rotationOffset.y = 0;
        float lookDirection = Vector3.SignedAngle(model.forward, rotationOffset, Vector3.up);
        anim.SetFloat("LookDirection", lookDirection);
        if(anim.GetFloat("Speed") > .1f)
        {
            model.forward += Vector3.Lerp(model.forward, rotationOffset, Time.deltaTime * rotationSpeed);
        }
    }

    private void HandleInputData()
    {
        anim.SetFloat("Speed", Vector3.ClampMagnitude(stickDirection, 1).magnitude, turnSpeed, Time.deltaTime); // clamp para limitar a 1, visto que a diagonal seria de 1.4
        anim.SetFloat("Horizontal", stickDirection.x);
        anim.SetFloat("Vertical", stickDirection.z);

        isWeaponEquipped = anim.GetBool("IsWeaponEquipped");
        isTargetLocked = anim.GetBool("IsTargetLocked");
        canRotate = anim.GetBool("CanRotate");

        if(isWeaponEquipped && Input.GetKeyDown(KeyCode.Space)) // locka o target apenas quando esta com arma equipada
        {
            anim.SetBool("IsTargetLocked", !isTargetLocked);
            isTargetLocked = !isTargetLocked;
        }

        if (Input.GetKeyDown(KeyCode.F) && !anim.GetBool("IsAttacking"))
        {
            anim.SetBool("IsWeaponEquipped", !isWeaponEquipped);
            isWeaponEquipped = !isWeaponEquipped;
            if(isWeaponEquipped == false)
            {
                anim.SetBool("IsTargetLocked", false);
                isTargetLocked = false;
            }
        }
    }

}
