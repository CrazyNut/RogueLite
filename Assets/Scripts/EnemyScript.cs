using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class EnemyScript : MonoBehaviour
{
    public Transform WeaponPosition;
    public float speed;
    public Rigidbody2D rb;
    public Vector2 moveVelocity;
    private float health;
    public float MaxHealth;
    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if(value < health)
            {
                BeforeAttackTimerOn = true;
            }
            health = value;
            if (health > MaxHealth)
                health = MaxHealth;
            if (health <= 0)
            {
                Die();
            }
            HealthBar.UpdateVals(health, MaxHealth);
        }
    }
    public WeaponObject weapon;
    public weaponScript weaponScript;
    public float VisRadius;
    public bool waitingAttackEnd;
    public Vector3 Direction;
    PersonController personPlayer;
    public float RetreatSpeed;
    public BarController HealthBar;
    public bool isLookingRight = true;
    public Vector3 ForceVector;
    public bool BeforeAttackTimerOn = true;
    public bool CorutineWaitBeforeAttackStarted = false;
    public float LerpVal;
    public float LerpIncr = 0.1f;
    void Start()
    {
        HealthBar = transform.Find("HealthBar").GetComponent<BarController>();
        health = MaxHealth;
        HealthBar.UpdateVals(health, MaxHealth);
        personPlayer = null;
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Direction = GetNearestPlayerDir();
        if (waitingAttackEnd && weaponScript.attack == false)
        {
            waitingAttackEnd = false;
        }
        moveVelocity = Direction * speed;
        if (!weaponScript.canAttack)
        {
            moveVelocity = Direction * RetreatSpeed * -1;
        }
        if(personPlayer != null)
        {
            if (personPlayer.transform.position.x < transform.position.x && isLookingRight)
            {
                isLookingRight = false;
                this.transform.Rotate(new Vector3(0, 180, 0));
                HealthBar.transform.localPosition = new Vector3(HealthBar.transform.localPosition.x + HealthBar.transform.localScale.x, HealthBar.transform.localPosition.y, 0);
                HealthBar.transform.Rotate(new Vector3(0, 180, 0));
            }
            if (personPlayer.transform.position.x > transform.position.x && !isLookingRight)
            {
                isLookingRight = true;
                this.transform.Rotate(new Vector3(0, -180, 0));
                HealthBar.transform.Rotate(new Vector3(0, -180, 0));
                HealthBar.transform.localPosition = new Vector3(HealthBar.transform.localPosition.x - HealthBar.transform.localScale.x, HealthBar.transform.localPosition.y, 0);
            }
        }
        
    }

    void FixedUpdate()
    {
        if (LerpVal >= 1.0f)
        {
            LerpVal = 1.0f;
        }
        else
        {
            LerpVal += LerpIncr;
            transform.GetComponent<Rigidbody2D>().AddForce(Vector3.Lerp(ForceVector, Vector3.zero, LerpVal));
        }

        rb.MovePosition(rb.position + moveVelocity * LerpVal * Time.deltaTime);
    }

    public void AddForce(Vector3 vector)
    {
        ForceVector = vector;
        LerpVal = 0.0f;
    }

    private void Die()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<WorldScript>().EnemiesInRoom--;
        GameObject.Destroy(this.gameObject);
    }

    public Vector3 GetNearestPlayerDir()
    {
        float dist = 10000f;
        Vector3 pos = Vector3.zero;
        Vector3 heading = Vector3.zero;
        if (personPlayer != null)
        {
            heading = personPlayer.transform.position - transform.position;
            dist = heading.magnitude;
            pos = personPlayer.transform.position;
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            heading = player.transform.position - transform.position;
            if(heading.magnitude < dist && heading.magnitude < VisRadius)
            {
                dist = heading.magnitude;
                pos = player.transform.position;
                personPlayer = player.GetComponent<PersonController>();
            }
        }


        if (!weaponScript.canAttack && !weaponScript.attack)
        {
            return heading / dist;
        }
        if (dist < weapon.BOTAttackRadius && !waitingAttackEnd)
        {
            if(BeforeAttackTimerOn == false)
            {
                BeforeAttackTimerOn = true;
                weaponScript.Attack(personPlayer, null, heading / dist);
                waitingAttackEnd = true;
            }
            else
            {
                StartCoroutine(WaitBeforeAttack());
            }
            return Vector3.zero;
        }else if (dist < weapon.BOTAttackRadius && waitingAttackEnd)
        {
            return Vector3.zero;
        }

        return heading / dist;
    }

    IEnumerator WaitBeforeAttack()
    {
        if (!CorutineWaitBeforeAttackStarted)
        {
            CorutineWaitBeforeAttackStarted = true;
            yield return new WaitForSeconds(weapon.BOTTimeBeforeAttack);
            BeforeAttackTimerOn = false;
            CorutineWaitBeforeAttackStarted = false;
        }
        else
        {
            yield return null;
        }
    }
}
