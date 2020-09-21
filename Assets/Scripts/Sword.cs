using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour {
    public static bool[] sword_hit = new bool[3];
    public BoxCollider bcol;
	// Use this for initialization
	void Start () {
        //bcol = gameObject.GetComponent<BoxCollider>();
		for (int i = 0; i < sword_hit.Length; i++)
        {
            sword_hit[i] = false;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (MarioController.anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            bcol.enabled = true;
        }
        else
        {
            bcol.enabled = false;
        }

    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.name.Contains("Bunny"))
        {
            sword_hit[0] = true;
        }
        else if (col.name.Contains("Fox"))
        {
            sword_hit[1] = true;

        }
        else if (col.name.Contains("Tiger"))
        {
            sword_hit[2] = true;
        }
    }
}
