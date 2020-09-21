using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyController : MonoBehaviour {

    public Transform other;

    public int Speed = 5;
    public int magnitude = 100;
    float timer = 3f;

    private void Update()
    {
        Vector3 WhereIsTarget = other.position - transform.position;
        print(WhereIsTarget.ToString());

        if (WhereIsTarget.sqrMagnitude <= magnitude)
        {
            CancelInvoke("MoveAround");
            Speed = 5;

            //float angle = Mathf.Atan2(WhereIsTarget.y, WhereIsTarget.x) * Mathf.Rad2Deg - 90; -- For 2D
            //transform.rotation = Quaternion.Euler(0, 0, angle);

            transform.LookAt(other); //For 3D
        }
        else
        {
            Invoke("MoveAround", 0);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);

    }

    void MoveAround()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            Speed = Random.Range(0, 5);
            transform.Rotate(0, Random.Range(0,360), 0);
            timer = 3f;
        }
    }
}

//Script by Isaac O'Brien
