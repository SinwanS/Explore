using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Upgrade_Cotroller : MonoBehaviour {

    float TempAttack;
    public Text AttackPre;
    public Text AttackPost;
    float TempDefense;
    public Text DefensePre;
    public Text DefensePost;
    float TempHealth;
    public Text HealthPre;
    public Text HealthPost;
    float TempMana;
    public Text ManaPre;
    public Text ManaPost;
    public static int TempUpgrade;
    public Text UpgradeText;
    public Button ConfirmButton;

    // Use this for initialization
    void Start () {
        SetValues();
	}
	
	// Update is called once per frame
	void Update () {
        if (MarioController.UpgradePoints == 0)
        {
            ConfirmButton.enabled = false;
        }
        else { ConfirmButton.enabled = true; }
        if (MarioController.leveledUP)
        {
            SetValues();
            MarioController.leveledUP = false;
        }
        UpdateText();
	}
    public void SetValues()
    {
        TempAttack = MarioController.Attack;
        AttackPre.text = "" + TempAttack;
        AttackPost.text = AttackPre.text;
        TempDefense = MarioController.Defense;
        DefensePre.text = "" + TempDefense;
        DefensePost.text = DefensePre.text;
        TempMana = MarioController.ManaObtainable;
        ManaPre.text = "" + TempMana;
        ManaPost.text = ManaPre.text;
        TempHealth = MarioController.HealthObtainable;
        HealthPre.text = "" + TempHealth;
        HealthPost.text = HealthPre.text;
        TempUpgrade = MarioController.UpgradePoints;
        UpgradeText.text = "Upgrade Points: " + TempUpgrade;
    }
    void Upgrade(float DesiredStat, Text Post )
    {
        if (TempUpgrade >0)
        {
            TempUpgrade--;
            DesiredStat = DesiredStat +5;
            Post.text = "" + DesiredStat;
            UpgradeText.text = "Upgrade Points: " + TempUpgrade;

        }
    }
    public void UpgradeHealth()
    {
        if (TempUpgrade > 0)
        {
        Upgrade(TempHealth, HealthPost);
        TempHealth += 5;
        }
        Confirm();
    }
    public void UpgradeAttack()
    {
        if (TempUpgrade > 0)
        {
        Upgrade(TempAttack, AttackPost);
        TempAttack += 5;
        }
        Confirm();
    }
    public void UpgradeDefense()
    {
        if (TempUpgrade > 0)
        {
        Upgrade(TempDefense, DefensePost);
        TempDefense += 5;
        }
        Confirm();
    }
    public void UpgradeMana()
    {
        if (TempUpgrade > 0)
            {
        Upgrade(TempMana, ManaPost);
        TempMana += 5;
        }
        Confirm();
    }
    public void Confirm()
    {
        MarioController.Attack = TempAttack;
        MarioController.Defense = TempDefense;
        MarioController.ManaObtainable = TempMana;
        MarioController.HealthObtainable = TempHealth;
        MarioController.UpgradePoints = TempUpgrade;
        MarioController.MAttack = 1 / 3 * MarioController.ManaObtainable;
        SetValues();

    }
    void UpdateText()
    {
        UpgradeText.text = "Upgrade Points: " + TempUpgrade;
        HealthPost.text = "" + TempHealth;
        ManaPost.text = "" + TempMana;
        AttackPost.text = "" + TempAttack;
        DefensePost.text = "" + TempDefense;
    }
}
