using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Enemy_Controller_Battle : MonoBehaviour {
    [SerializeField]
    private Stat Ehealth;
    public static float[] EAttack = new float[4];
    public static float[] EDefense = new float[4];
    public static float[] EXPGive = new float[4];
    public static float[] ESpeed = new float[4];

    public static int E_Death_Count = 0;
    private int currentEnemy;

	// Use this for initialization
    void Awake()
    {
        Ehealth.Initialize();
    }
	void Start () {
        CheckCurrentEnemy();
        if (currentEnemy == 0)
        {
        Enemy1();
        }
        if (currentEnemy == 1)
        {
            Enemy2();
        }
        if (currentEnemy == 2)
        {
            Enemy3();
        }
        if (currentEnemy == 3)
        {
            Enemy4();
        }

    }
	
	// Update is called once per frame
	void Update () {
        if (Ehealth.CurrentVal == 0)
        {
            MarioController.CE = currentEnemy;
            if(gameObject.name == "clock")
            {
                E_Death_Count++;
                GuiCode.score = 1000 + (int)MarioController.ActualExp;
                SceneManager.LoadScene("end");
            }
            E_Death_Count++;
            Destroy(gameObject);
        }
	}
    void Enemy1()
    {
        EAttack[0] = 20;
        EDefense[0] = 5;
        Ehealth.MaxValue = 20;
        Ehealth.CurrentVal = 20;
        EXPGive[0] = 10;
    }
    void Enemy2()
    {
        EAttack[1] = 30;
        EDefense[1] = 10;
        Ehealth.MaxValue = 45;
        Ehealth.CurrentVal = 45;
        EXPGive[1] = 30;
    }
    void Enemy3()
    {
        EAttack[2] = 60;
        EDefense[2] = 50;
        Ehealth.MaxValue = 100;
        Ehealth.CurrentVal = 100;
        EXPGive[2] = 100;
    }
    void Enemy4()
    {
        EAttack[3] = 25;
        EDefense[3] = 5;
        Ehealth.MaxValue = 500;
        Ehealth.CurrentVal = 500;
        EXPGive[3] = 1000;
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.name.Equals("Sword"))
        {
            Sword_Attack();
        }
        if (col.name.Contains("Energy"))
        {
            Magic_Attack();
        }
    }
    void Sword_Attack()
    {
        if (MarioController.equippedMask[2])
        {
            float Damage = MarioController.SAttack * MarioController.weaponcoefficient - EDefense[currentEnemy];
            if (Damage > 0)
            {
                Ehealth.CurrentVal -= Damage;
            }
            else
            {
                Ehealth.CurrentVal--;
            }
        }
        else {
            float Damage = MarioController.Attack * MarioController.weaponcoefficient - EDefense[currentEnemy];
            if (Damage > 0)
            {
            Ehealth.CurrentVal -= Damage;
            }
            else
            {
            Ehealth.CurrentVal--;
            }
        }
        for (int i = 0; i < 3; i++)
        {
            Sword.sword_hit[i] = false;
        }
    }
    public void Magic_Attack()
    {
        if (MarioController.equippedMask[1])
        { 
            float Damage = MarioController.SMAttack- EDefense[currentEnemy];
            print(Damage);
            if (Damage > 0)
            {
                Ehealth.CurrentVal -= Damage;
            }
            else
            {
                Ehealth.CurrentVal--;
            }
        }
        else {
            float Damage = MarioController.MAttack-EDefense[currentEnemy];
            if (Damage > 0)
            {
            Ehealth.CurrentVal -= Damage;
            }
            else
            {
            Ehealth.CurrentVal--;
            }
            print(Damage);
        }
        for (int i = 0; i < 3; i++)
        {
            Sword.sword_hit[i] = false;
        }
    
    }
    public void CheckCurrentEnemy()
    {
        if (this.name.Contains("Bunny"))
        {
            currentEnemy = 0;
        }
        else if (this.name.Contains("Fox"))
        {
            currentEnemy = 1;
        }
        else if (this.name.Contains("Tiger"))
        {
            currentEnemy = 2;
        }
        else { currentEnemy = 3; }
    }
}
