using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerr : MonoBehaviour
{

    public Transform other;
    Rigidbody rb;

    public int MaxSpeed;
    public float Speed = 5;
    public int magnitude = 100;
    public int animMagnitude;

    public float jumpHeight = 170;
    float ogJumpHeight = 170;

    bool isGrounded = true;
    int timeGrounded = 0;

    public float originalTimer = 5f;
    float timer;
    public float tTimer = 1f;
    float tigerT;

    public GameObject beam;
    public float timeBetweenShots;
    public float shotCounter;
    public Transform firePoint;

    Animator anim;

    GameObject Player;
    Transform player;

    public BoxCollider bcolBunny;
    public BoxCollider bcolTiger;

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = originalTimer;
        tigerT = tTimer;
        rb = gameObject.GetComponent<Rigidbody>();
        Player = GameObject.FindGameObjectWithTag("Player");
        other = GameObject.FindGameObjectWithTag("Player").transform;
        player = Player.transform;
        jumpHeight = ogJumpHeight;
    }

    private void Update()
    {
        if (shotCounter > 0)
        {
            shotCounter -= Time.deltaTime;
        }

        bool invoke = false;
        Vector3 WhereIsTarget = other.position - transform.position;

        if (transform.name.Contains("Bunny"))
        {
            Invoke("Jump", 3f);
        }

        if (WhereIsTarget.sqrMagnitude < animMagnitude)
        {
            if (transform.name.Contains("Bunny"))
            {
                bcolBunny.enabled = true;
            }
            if (transform.name.Contains("Tiger"))
            {
                bcolTiger.enabled = true;
            }
            anim.SetBool("attack", true);
        }
        else
        {
            if (transform.name.Contains("Bunny"))
            {
                bcolBunny.enabled = false;
            }
            if (transform.name.Contains("Tiger"))
            {
                bcolTiger.enabled = false;
            }
            anim.SetBool("attack", false);
        }

        if (transform.name.Contains("Bunny") && WhereIsTarget.sqrMagnitude <= magnitude)
        {
            invoke = false;
            Speed = 5;
            transform.LookAt(player);
            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            transform.rotation = q;
        }
        else
        {
            if (!invoke)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Speed = Random.Range(1, 5);
                    Quaternion q = transform.rotation;
                    q.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    transform.rotation = q;
                    timer = originalTimer;
                }
            }
            invoke = true;
        }

        if (transform.name.Contains("Tiger") && WhereIsTarget.sqrMagnitude <= magnitude)
        {
            if (invoke == true)
            {
                Sprint();
            }

            invoke = false;
            transform.LookAt(player);
            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            transform.rotation = q;
        }
        else
        {
            if (!invoke)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Speed = Random.Range(1, 5);
                    Quaternion q = transform.rotation;
                    q.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    transform.rotation = q;
                    timer = originalTimer;
                }
            }
            invoke = true;
        }

        if (transform.name.Contains("Fox") && WhereIsTarget.sqrMagnitude <= magnitude)
        {
            if (invoke == true)
            {
                if (shotCounter < 0)
                {
                    shotCounter = timeBetweenShots;
                    transform.LookAt(player);
                    Fire();
                }
            }
            invoke = false;
            Speed = -2;
            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(q.eulerAngles.x, q.eulerAngles.y, 0);
            transform.rotation = q;
        }
        else
        {
            if (!invoke)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    Speed = Random.Range(1, 5);
                    Quaternion q = transform.rotation;
                    q.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    transform.rotation = q;
                    timer = originalTimer;
                }
            }
            invoke = true;
        }

        transform.Translate(Vector3.forward * Time.deltaTime * Speed);

    }

    void Sprint()
    {
        tigerT -= Time.deltaTime;
        if (Speed >= MaxSpeed)
        {
            Speed = 1;
        }
        else if (tigerT <= 0)
        {
            print(Speed);
            Speed += 2;
            tigerT = tTimer;
        }
    }

    void Jump()
    {
        if (timeGrounded <= 10)
        {
            if (isGrounded)
            {
                jumpHeight *= 1f;
            }
        }
        else
        {
            jumpHeight = ogJumpHeight;
        }
        if (isGrounded)
        {
            isGrounded = false;
            rb.AddForce(0, jumpHeight, 0);
        }
    }

    void Fire()
    {
        anim.SetBool("attack", true);
        Instantiate(beam, firePoint.position, firePoint.rotation);
        anim.SetBool("attack", false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isGrounded) timeGrounded = 0;
        timeGrounded++;
        isGrounded = true;
    }

    void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }


}
