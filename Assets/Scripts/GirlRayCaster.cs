using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GirlRayCaster : MonoBehaviour
{
    [HideInInspector]
    public string IamOver = "Sand";
    void Update()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.transform.position + Vector3.up, Vector3.down);
        //Debug.DrawRay(transform.position, Vector3.down, Color.green);

        if (Physics.Raycast(ray, out hit, 5))
        {
            if (hit.transform.tag == "Walkable")
            {
                if (hit.transform.name == "SandPlane")
                {
                    IamOver = "Sand";
                }
                else
                {
                    IamOver = "Stone";
                }
            }
        }
    }
}
