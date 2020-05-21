using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hittable : MonoBehaviour
{
    public void Hit(HitData data)
    {
        Debug.Log(data.id);
    }
}
