using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscScale : MonoBehaviour
{
    float scalingFactor = 1.025f;
    float time = 0;
    //public BoxCollider bcol;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.localScale = new Vector3(transform.localScale.x * scalingFactor, transform.localScale.y * scalingFactor, 2);
        if(time>= 3)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider col)
    {
        //if(col == bcol)
        //{
        //    MarioController.imBad = true;
        //}
    }

}
