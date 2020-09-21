using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Teleport : MonoBehaviour
{
    public static bool inCombatScene = false;
    public static int startdeadenemies = 0;
    int enddeadenemies = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (inCombatScene)
        {
            if (Input.GetAxis("TPHome") > .01 || Input.GetKey(KeyCode.H))
            {
                inCombatScene = false;
                SceneManager.LoadScene("main");
            }
        }
        if (target.count == 7)
        {
            target.count = 0;
            SceneManager.LoadScene("main");
        }
        enddeadenemies = Enemy_Controller_Battle.E_Death_Count + 1;
        if (startdeadenemies != 0 && (enddeadenemies - startdeadenemies == 10))
        {
            startdeadenemies = 0;
            enddeadenemies = 0;
            SceneManager.LoadScene("main");
        }
        
    }
    void OnTriggerEnter(Collider col)
    {
        if (gameObject.name.Equals("Jump"))
        {
            if (MarioController.hasMasks[0] == true)
            {
                SceneManager.LoadScene("main");
                return;
            }
            MarioController.trippleJump = true;
            SceneManager.LoadScene("Platforms");
        }
        if (gameObject.name.Equals("Magic"))
        {
            if (MarioController.hasMasks[1] == true)
            {
                SceneManager.LoadScene("main");
                return;
            }
            MarioController.magicAttack = true;
            SceneManager.LoadScene("MagicRoom");
        }
        if (gameObject.name.Equals("Fight"))
        {
            inCombatScene = true;
            SceneManager.LoadScene("Combat");
        }
        if (gameObject.name.Equals("Sword"))
        {
            if (MarioController.hasMasks[2] == true)
            {
                SceneManager.LoadScene("main");
                return;
            }
            startdeadenemies = Enemy_Controller_Battle.E_Death_Count + 1;
            MarioController.betterSword = true;
            SceneManager.LoadScene("Sword");
        }
        if (gameObject.name.Equals("PlatformingTP"))
        {
            SceneManager.LoadScene("main");
        }
        if (gameObject.name.Equals("Boss"))
        {
            SceneManager.LoadScene("Boss");
        }
    }
}
