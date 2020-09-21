using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFight : MonoBehaviour
{
    public GameObject prefab;
    float time = 0;
    public Transform other;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookPos = other.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 100);
        time += Time.deltaTime;

        if(time >= 2)
        {
            time = 0;
            int t = Random.Range(0, 2);
            if(t == 0)
            {
                Enemy_Controller_Battle.EAttack[3] = 25;
                Instantiate(prefab, new Vector3(transform.position.x,transform.position.y - 6f,transform.position.z), Quaternion.Euler(new Vector3(90, 0, 0)));
            }
            else
            {
                Enemy_Controller_Battle.EAttack[3] = 20;
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0,0,0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 45, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 90, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 135, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 225, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 270, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 315, 0)));
                Instantiate(prefab, transform.position, Quaternion.Euler(new Vector3(0, 360, 0)));
            }
        }
    }
}
