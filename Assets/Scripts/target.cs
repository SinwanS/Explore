using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class target : MonoBehaviour
{
    Light glo;
    public static int count;
    BoxCollider bcol;
    // Start is called before the first frame update
    void Start()
    {
        glo = GetComponent<Light>();
        bcol = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name.Contains("Beam"))
        {
            bcol.enabled = false;
            glo.enabled = true;
            count++;
        }
    }
}