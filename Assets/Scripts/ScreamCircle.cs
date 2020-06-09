using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreamCircle : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, 5);
    }

    private void Update()
    {
        this.transform.localScale += new Vector3(30f, 30f, 30f) * Time.deltaTime;
    }
}
