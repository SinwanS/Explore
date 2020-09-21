using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mask_Controller : MonoBehaviour
{
    public Button ButtonT;
    public Button ButtonF;
    public Button ButtonB;
    public GameObject MB;
    public GameObject MF;
    public GameObject MT;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        SetBActive();
        ButtonB.image.enabled = MarioController.hasMasks[0];
        ButtonF.image.enabled = MarioController.hasMasks[1];
        ButtonT.image.enabled = MarioController.hasMasks[2];
        MB.GetComponent<MeshRenderer>().enabled = MarioController.equippedMask[0];
        MF.GetComponent<MeshRenderer>().enabled = MarioController.equippedMask[1];
        MT.GetComponent<MeshRenderer>().enabled = MarioController.equippedMask[2];
    }
    public void SetBunny()
    {
        MarioController.equippedMask[0] = true;
        MarioController.equippedMask[1] = false;
        MarioController.equippedMask[2] = false;
    }
    public void SetFox()
    {
        MarioController.equippedMask[0] = false;
        MarioController.equippedMask[1] = true;
        MarioController.equippedMask[2] = false;
    }
    public void SetTiger()
    {
        MarioController.equippedMask[0] = false;
        MarioController.equippedMask[1] = false;
        MarioController.equippedMask[2] = true;
    }
    public void SetBActive()
    {
        if (MarioController.hasMasks[0] == false) { ButtonB.enabled = false; } else { ButtonB.enabled = true; }
        if (MarioController.hasMasks[1] == false) { ButtonF.enabled = false; } else { ButtonB.enabled = true; }
        if (MarioController.hasMasks[2] == false) { ButtonT.enabled = false; } else { ButtonB.enabled = true; }
    }
}
