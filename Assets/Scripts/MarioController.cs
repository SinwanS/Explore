using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MarioController : MonoBehaviour
{

    // Stats
    [SerializeField]
    private Stat Health;
    public static float HealthObtainable = 100;
    public static float LastHealth = 100;
    [SerializeField]
    private Stat Mana;
    public static float ManaObtainable = 100;
    public static float LastMana = 100;
    // Exp and Level
    [SerializeField]
    private Stat Exp;
    public static float ActualExp = 0;
    private float[] Expcap = new float[51];
    public static int UpgradePoints = 0;
    public Text Exp_Text;
    public Text Level_Text;
    public static int level = 1;
    // Basic Stats
    public static float Attack = 10;
    public static float Defense = 10;
    // Weapons
    public static float weaponcoefficient = 1;
    // IDK
    public static int CE = 0;
    public static int Death_Counter = 0;
    // Masks
    public static bool[] equippedMask = new bool[3];
    public static bool[] hasMasks = new bool[3];
    public static float SAttack;
    public static float MAttack;
    public static float SMAttack;
    public static float SDefense;
    public static bool leveledUP;
    public static float Svelocity;
    public static float SJump;
    // Magic
    public static bool[] MagicHit = new bool[3];

    public float velocity;
    public float turnSpeed;
    public float ogJumpHeight;
    float jumpHeight;
    Vector2 input;
    float angle;
    public float dodgeSpeed;
    public float dodgeTime = .6f;
    public GameObject[] enemies;
    public List<Transform> enemyTransforms;
    public static bool locked = false;
    public Transform center;

    public static bool CanDodge = true;
    public bool Dodging = false;

    public GameObject beam;
    public float timeBetweenShots;
    public float shotCounter;
    public Transform firePoint;


    Quaternion targetRotation;
    Transform cam;

    Vector3 playerpos = new Vector3();

    bool isGrounded = true;
    int timeGrounded = 0;
    int numJumps = 0;
    Rigidbody rb;

    public static bool trippleJump = false;
    public static bool magicAttack = false;
    public static bool betterSword = false;

    string agility;

    public static Animator anim;

    //public static bool imBad;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Health.Initialize();
        Mana.Initialize();
        Exp.Initialize();
    }

    // Use this for initialization
    void Start()
    {
        CanDodge = true;
        CanDodge = true;
        jumpHeight = ogJumpHeight;
        cam = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyTransforms.Add(enemies[i].transform);
        }
        for (int i = 0; i < Expcap.Length; i++)
        {
            Expcap[i] = i * i + i;
        }
        hasMasks[0] = true; hasMasks[1] = true; hasMasks[2] = true;
        StaticUIElements();
        Health.CurrentVal = LastHealth;
        Mana.CurrentVal = LastMana;
    }
    // Update is called once per frame
    void Update()
    {
        MAttack = 1 / 3f * Mana.MaxValue;
        MaskPowerUps();
        StaticUIElements();
        LastHealth = Health.CurrentVal;
        LastMana = Mana.CurrentVal;
        hasMasks[0] = trippleJump;
        hasMasks[1] = magicAttack;
        hasMasks[2] = betterSword;
        if (isGrounded)
        {
            playerpos = transform.position;
        }

        if (betterSword)
        {
            weaponcoefficient = 2;
        }
        IsDead();
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = null;
        }
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyTransforms.Add(enemies[i].transform);
        }
        if (!GuiCode.isPaused)
        {
            //if (!isGrounded)
            //{
            //    timeGrounded = 0;
            //}
            //else
            //{
            //    timeGrounded++;
            //}
            ManaRegen();
            //LockOn();
            //Dodge();
            GetInput();
            if (shotCounter > 0)
            {
                shotCounter -= Time.deltaTime;
            }

            if (Mathf.Abs(input.x) < .01 && Mathf.Abs(input.y) < .01)
            {
                anim.SetBool("running", false);
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                return;
            }
            else
            {
                anim.SetBool("running", true);
            }
            Rotate();

            if (!isGrounded)
            {
                rb.AddForce(-transform.up * 9.8f);
            }
            Move();
            
        }
    }

    void GetInput()
    {

        if (Input.GetButtonDown("Jump"))
        {
            if (trippleJump)
            {
                if (timeGrounded <= 10 && numJumps > 0 && numJumps < 3)
                {
                    if (isGrounded)
                    {
                        jumpHeight *= 1.5f;
                    }
                }
                else
                {
                    jumpHeight = ogJumpHeight;
                    numJumps = 0;
                }
            }
            if (isGrounded)
            {
                isGrounded = false;
                rb.AddForce(0, jumpHeight, 0);
                numJumps++;
            }
        }

        if (Input.GetButton("Fire1"))
        {
            if (shotCounter < 0)
            {
                shotCounter = timeBetweenShots;
                Fire();
            }

        }

        if (Input.GetButton("Attack"))
        {
            anim.SetBool("attacking", true);
        }
        else if (Input.GetButtonUp("Attack"))
        {
            anim.SetBool("attacking", false);
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

    }

    void Fire()
    {
        if (magicAttack && Mana.CurrentVal >= 20)
        {
            Instantiate(beam, firePoint.position, firePoint.rotation);
            Mana.CurrentVal -= 20;
        }
    }

    void Rotate()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    void Move()
    {
        //if (input.y < 0)
        //{
        //    transform.position += -transform.forward * velocity * Time.deltaTime;
        //}
        //else
        //{
        //    transform.position += transform.forward * velocity * Time.deltaTime;
        //}
        Vector3 right = Vector3.zero;
        Vector3 forward = Vector3.zero;
            if (Input.GetAxis("Horizontal") > 0)
            {
                right = transform.forward * Input.GetAxis("Horizontal") * velocity;
            }
            else
            {
                right = -transform.forward * Input.GetAxis("Horizontal") * velocity;
            }
            if (Input.GetAxis("Vertical") > 0)
            {
                forward = transform.forward * Input.GetAxis("Vertical") * velocity;
            }
            else
            {
                forward = -transform.forward * Input.GetAxis("Vertical") * velocity;
            }
        //Vector3 right1 = Vector3.zero;
        //Vector3 forward1 = Vector3.zero;
        //if (Input.GetAxis("Horizontal") > 0)
        //{
        //    //right = transform.forward * Input.GetAxis("Horizontal") * 5;
        //    right1 = new Vector3(5, 0, 0);
        //}
        //else
        //{
        //    //right = -transform.forward * Input.GetAxis("Horizontal") * 5;
        //    right1 = new Vector3(-5, 0, 0);
        //}
        //if (Input.GetAxis("Vertical") > 0)
        //{
        //    //forward = transform.forward * Input.GetAxis("Vertical") * 5;
        //    forward1 = new Vector3(0, 0, 2);
        //}
        //else
        //{
        //    //forward = -transform.forward * Input.GetAxis("Vertical") * 5;
        //    forward1 = new Vector3(0, 0, -2);
        //}
        //right = new Vector3(0, 0, rb.velocity.z);
        //forward = new Vector3(rb.velocity.x, 0, 0);
        //right += right1;
        //forward += forward1;
    

        Vector3 down = new Vector3(0, rb.velocity.y, 0);
        rb.velocity = Vector3.ClampMagnitude(right + forward + down, 30);
        //rb.velocity = new Vector3(Input.GetAxis("Horizontal")*velocity, rb.velocity.y, Input.GetAxis("Vertical")*velocity);
    }
    void OnTriggerStay(Collider col)
    {
        if (!isGrounded) timeGrounded = 0;
        timeGrounded++;
        isGrounded = true;
        if (col.gameObject.name.Equals("clock"))
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name.Equals("TPBACK"))
        {
            transform.position = playerpos;
        }
    }

        void OnCollisionExit(Collision col)
    {
        if(col.gameObject.tag.Equals("Damage"))
        {
            print("Hit");
            if (equippedMask[2] == false)
            {
                if (col.gameObject.name.Contains("Ring"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[3] - Defense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
            }

            else
            {
                if (col.gameObject.name.Contains("Ring"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[3] - SDefense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }

                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        isGrounded = false;
        if (col.tag.Equals("Damage"))
        {
            if(equippedMask[2]== false)
            {
                if (col.name.Contains("Bunny"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[0] - Defense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Fox"))
                   {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[1] - Defense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Tiger"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[2] - Defense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Disc"))
                {
                    print("hey");
                    float Enemydamage = Enemy_Controller_Battle.EAttack[3] - Defense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
            }
            else
            {
                if (col.name.Contains("Bunny"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[0] - SDefense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Fox"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[1] - SDefense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Tiger"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[2] - SDefense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
                if (col.name.Contains("Disc"))
                {
                    float Enemydamage = Enemy_Controller_Battle.EAttack[3] - SDefense;
                    if (Enemydamage > 0)
                    {
                        Health.CurrentVal -= Enemydamage;
                    }
                    else
                    {
                        Health.CurrentVal -= 1;
                    }
                }
            }
        }
    }
    //void Dodge()
    //{
    //    if (CanDodge)
    //    {
    //        if (Input.GetButton("Dodge"))
    //        {
    //            StartCoroutine(DodgeC());
    //            StartCoroutine(DodgeW());
    //        }
    //    }
    //}

    //IEnumerator DodgeC()
    //{
    //    CanDodge = false;
    //    yield return new WaitForSeconds(.75f);
    //    CanDodge = true;
    //}

    //IEnumerator DodgeW()
    //{
    //    Dodging = true;
    //    rb.velocity += transform.forward * dodgeSpeed;
    //    yield return new WaitForSeconds(dodgeTime);
    //    Dodging = false;
    //    rb.velocity = new Vector3(0, rb.velocity.y, 0);
    //}
    void LockOn()
    {
        if (Mathf.Abs(Input.GetAxisRaw("Lock On")) >= .01)
        {
            foreach (Transform tf in enemyTransforms)
            {
                Vector3 relative = transform.InverseTransformPoint(tf.position);

                if (relative.z > 0 && Mathf.Abs(relative.x) < 15)
                {
                    locked = true;
                    var lookPos = tf.position - transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10);
                    //center.rotation = Quaternion.Slerp(center.rotation, Quaternion.LookRotation(center.position - transform.position), Time.deltaTime * 10);
                    center.rotation = transform.rotation;
                   // MagicController.enen = tf;
                }
                locked = false;
            }
        }
    }
    void LevelUp()
    {
        if (ActualExp >= Expcap[level] && level != 50)
        {
            leveledUP = true;
            level++;
            Exp.MaxValue = Expcap[level] - Expcap[level - 1];
            UpgradePoints += 4;
        }
    }
    void StaticUIElements()
    {
        Health.MaxValue = HealthObtainable;
        Mana.MaxValue = ManaObtainable;
        Health.CurrentVal = Health.CurrentVal;
        Mana.CurrentVal = Mana.CurrentVal;
        ActualExp = Mathf.Clamp(ActualExp, 0, 2500);
        string[] temp = Level_Text.text.Split(':');
        Level_Text.text = temp[0] + ": " + level;
        Exp.CurrentVal = ActualExp - Expcap[level - 1];
        Exp.MaxValue = Expcap[level] - Expcap[level - 1];
        Exp_Text.text = "Exp: " + ActualExp + "/" + Expcap[level];
        if (Enemy_Controller_Battle.E_Death_Count != Death_Counter)
        {
            for (int i = 0; i < Mathf.RoundToInt(Enemy_Controller_Battle.EXPGive[CE]); i++)
            {
                ActualExp++;
                LevelUp();
            }
            LastMana = Mana.CurrentVal;
            LastHealth = Health.CurrentVal;
            Death_Counter++;
        }
    }
    public void AddHP()
    {
        if(UpgradePoints > 0 && Health.CurrentVal < Health.MaxValue)
        {
            Health.CurrentVal = Health.MaxValue;
            UpgradePoints -= 1;
            leveledUP = true;
        }
    }
    public void IsDead()
    {
        if(Health.CurrentVal == 0)
        {
            SceneManager.LoadScene("end");
        }
    }
    public void ManaRegen(){
     if(equippedMask[1]){
        Mana.CurrentVal += +.08f * Mana.MaxValue * Time.deltaTime;
        }
     else
        {
        Mana.CurrentVal += +.04f * Mana.MaxValue * Time.deltaTime;
        }
    }
    public void MaskPowerUps()
    {
        SAttack = 1.3f * Attack;
        SDefense = 1.3f * Defense;
        Svelocity =2f * velocity;
        SJump = 1.5f * jumpHeight;
        SMAttack = 1.3f * MAttack;
    }
}