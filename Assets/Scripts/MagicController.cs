using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour {

    public float speed;
    public float life;
    Rigidbody rb;
    
    //public static Transform enen;
    // Use this for initialization
    void Start () {
        //transform.LookAt(enen);
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        rb.velocity = transform.forward * speed * Time.deltaTime;
        life -= Time.deltaTime;
        if (life <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        //Destroy(gameObject);
    }
}
