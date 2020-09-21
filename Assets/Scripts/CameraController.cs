using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    Vector2 Mouse;
    float OGRotationX = 21.5f;
    Quaternion targetRotation;
    float turnSpeed=90;
	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(GameObject.FindGameObjectWithTag("Player").transform.position.x, (5.77f+ GameObject.FindGameObjectWithTag("Player").transform.position.y), (-1.15f+ GameObject.FindGameObjectWithTag("Player").transform.position.z));
        Mouse = new Vector2(Input.GetAxisRaw("Mouse X"), Mathf.Clamp(Input.GetAxisRaw("Mouse Y"),-1,1));
        if(Mathf.Abs(Mouse.x)>.01)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * Mouse.x * turnSpeed, Space.World);
        }
        if (Mathf.Abs(Mouse.y)> .1)
        {
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.Euler(Mathf.Clamp(45*Mouse.y,-10,10)+21.5f,transform.rotation.y,transform.rotation.z), 10 * Time.deltaTime);
        }
        else
        {
            Camera.main.transform.localRotation = Quaternion.Slerp(Camera.main.transform.localRotation, Quaternion.Euler(OGRotationX, transform.rotation.y, transform.rotation.z), 10 * Time.deltaTime);
        }
    }
}
