using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excluir : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print(this.gameObject.name + ": " + transform.childCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
