using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEffectScript : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(this.gameObject, 4);
    }
}
