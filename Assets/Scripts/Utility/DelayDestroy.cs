using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayDestroy : MonoBehaviour
{
    public float destroyDelay;

    void Update()
    {
        Destroy(gameObject, destroyDelay);
    }
}
