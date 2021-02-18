
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class PersonController : MonoBehaviour
{
    public float speed;
    public float RoadSpeed;
    public Rigidbody2D rb;
    public Vector2 moveVelocity;
    public JoyButton AttackButton;
    public string weaponName;
    public WeaponObject weaponData;
    public Transform WeaponPos;
    public weaponScript weaponScr;
    public Joystick joystick;
    private float health;
    public float MaxHealth;
    public bool UsersPlayer;
    private PlayerHealthBar healthBar;
    public Vector3 ForceVector;
    public float LerpVal;
    public float LerpIncr = 0.1f;
    public GameObject Enemy;
    public MapGenerator mg;
    public Vector3 PortalTarget;
    public int PortalTargetId;
    public bool teleport;
    public Animator anim;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            if (health > MaxHealth)
                health = MaxHealth;
            if (health <= 0)
                Die();

            healthBar.UpdateVals(health, MaxHealth);
        }
    }
    public bool IsLookingRight = true;
    void Start()
    {
        anim = this.GetComponent<Animator>();
        mg = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>();
        Enemy = null;
        weaponData = WeaponObject.GetWearponByName(weaponName, GameObject.FindGameObjectWithTag("Map").GetComponent<MapGeneratorDataScript>().wearpons);
        weaponScr = GameObject.Instantiate(weaponData.obj, WeaponPos.position, Quaternion.identity).GetComponent<weaponScript>();
        weaponScr.transform.parent = WeaponPos;
        weaponScr.weaponData = weaponData;
        healthBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<PlayerHealthBar>();
        health = MaxHealth;
        rb = this.GetComponent<Rigidbody2D>();
        healthBar.InstVals(health, MaxHealth);
    }

    private void Update()
    {


        if (teleport)
        {
            anim.SetBool("BubbleEnabled", true);
            this.GetComponent<BoxCollider2D>().isTrigger = true;
            Vector3 heading = PortalTarget - transform.position;
            float dist = heading.magnitude;
            moveVelocity = heading / dist;
            if (dist < 0.2f)
            {
                GameObject.FindGameObjectWithTag("Map").GetComponent<WorldScript>().Teleported(PortalTargetId);
                teleport = false;
                anim.SetBool("BubbleEnabled", false);
                this.GetComponent<BoxCollider2D>().isTrigger = false;
            }
            return;
        }



        Vector2 moveInput = new Vector2(joystick.Horizontal, joystick.Vertical);
        if (!mg.Roads.HasTile(mg.Roads.WorldToCell(transform.position)))
        {
            moveVelocity = moveInput.normalized * speed;
        }
        else
        {
            moveVelocity = moveInput.normalized * RoadSpeed;
        }
        if(IsLookingRight && moveInput.x < 0)
        {
            IsLookingRight = false;
            transform.Rotate(new Vector3(0, 180, 0));
        }
        else if(!IsLookingRight && moveInput.x > 0)
        {
            IsLookingRight = true;
            transform.Rotate(new Vector3(0, -180, 0));
        }
        if (weaponData != null && (Input.GetKey(KeyCode.A) || AttackButton.Pressed))
        {
            SearchEnemies();
            if(Enemy != null)
            {
                if (!weaponData.SplashAttack)
                {
                    Vector3 heading = Enemy.transform.position - transform.position;
                    this.weaponScr.Attack(null, Enemy.GetComponent<EnemyScript>(), heading / heading.magnitude);
                }
                else
                {
                    Vector3 heading = Enemy.transform.position - transform.position;
                    this.weaponScr.AttackEnemies(nearbyEnemies(), heading / heading.magnitude);
                }
            }
            else
            {
                this.weaponScr.Attack(null, null, moveInput);
            }
        }
    }

    private List<EnemyScript> nearbyEnemies()
    {
        List<EnemyScript> res = new List<EnemyScript>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float tmp = Mathf.Sqrt(Mathf.Pow(transform.position.x - enemy.transform.position.x, 2) + Mathf.Pow(transform.position.y - enemy.transform.position.y, 2));
            if (tmp < weaponData.attackRadius)
            {
                res.Add(enemy.GetComponent<EnemyScript>());
            }
        }
        return res;
    }

    private bool SearchEnemies()
    {
        float distanse = 1000000f;
        GameObject newEnemy = null;
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float tmp = Mathf.Sqrt(Mathf.Pow(transform.position.x - enemy.transform.position.x, 2) + Mathf.Pow(transform.position.y - enemy.transform.position.y, 2));
            if(tmp < distanse && tmp < weaponData.attackRadius)
            {
                distanse = tmp;
                newEnemy = enemy;
            }
        }
        if (newEnemy != null && Enemy != newEnemy)
        {
            Enemy = newEnemy;
            return true;
        }
        if(Enemy != null)
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        if(LerpVal >= 1.0f)
        {
            LerpVal = 1.0f;
        }
        else
        {
            LerpVal += LerpIncr;
            transform.GetComponent<Rigidbody2D>().AddForce(Vector3.Lerp(ForceVector, Vector3.zero,LerpVal));
        }
        if(teleport)
            rb.MovePosition(rb.position + moveVelocity * RoadSpeed * LerpVal * Time.deltaTime); 
        else
            rb.MovePosition(rb.position + moveVelocity * LerpVal * Time.deltaTime);
    }

    private void Die()
    {

    }

    public void AddForce(Vector3 vector)
    {
        ForceVector = vector;
        LerpVal = 0.0f;
    }

}