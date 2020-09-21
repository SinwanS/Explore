using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    double time = 0;
    public GameObject Bunny;
    public GameObject Fox;
    public GameObject Tiger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (name.Contains("Bunny"))
        {
            if (time >= 5)
            {
                Instantiate(Bunny, transform.position, transform.rotation);
                time = 0;
            }
        }
        if (name.Contains("Fox"))
        {
            if (time >= 10)
            {
                Instantiate(Fox, transform.position, transform.rotation);
                time = 0;
            }
        }
        if (name.Contains("Tiger"))
        {
            if (time >= 30)
            {
                Instantiate(Tiger, transform.position, transform.rotation);
                time = 0;
            }
        }
    }
}
