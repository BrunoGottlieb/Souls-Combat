using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSwordFromSky : MonoBehaviour
{
    public float fallSpeed = 50;

    private void OnEnable()
    {
        Destroy(this.gameObject, 2);
    }

    void Update()
    {
        this.transform.position += transform.up * Time.deltaTime * fallSpeed * -1;
    }

}
