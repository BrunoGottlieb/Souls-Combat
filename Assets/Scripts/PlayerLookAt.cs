using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookAt : MonoBehaviour
{
    Animator anim;
    Camera mainCamera;

    void Start()
    {
        anim = this.GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetLookAtWeight(1f, 0.5f, 1f, 0.5f, 0.5f);
        Ray lookAtRay = new Ray(transform.position, mainCamera.transform.forward);
        anim.SetLookAtPosition(lookAtRay.GetPoint(25));
    }
}
