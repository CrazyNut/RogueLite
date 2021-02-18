using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 rotationVector;
    public Vector3 directionVector;
    public float speed;
    public Rigidbody2D rb;
    public float attackPower;
    public bool start = false;
    private Vector2 moveVelocity;
    private float bulletForce;
    public bool enemyBullet;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            moveVelocity = directionVector.normalized * speed;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyBullet && other.gameObject.tag == "Player")
        {
            other.GetComponent<PersonController>().Health -= attackPower;
            other.GetComponent<PersonController>().AddForce(directionVector * bulletForce);
            Die();
        }
        else if(!enemyBullet && other.gameObject.tag == "Enemy")
        {
            other.GetComponent<EnemyScript>().Health -= attackPower;
            other.GetComponent<EnemyScript>().AddForce(directionVector * bulletForce);
            Die();
        }
    }

    private void Die()
    {
        GameObject.Destroy(this.gameObject);
    }

    public void StartBullet(bool enemyBullet, Vector3 target, Vector3 directionVector, float speed, float deathTime, float attackPower, Vector2 RandomVec, float bulletForce)
    {
        StartCoroutine(DeathTimer(deathTime));
        this.enemyBullet = enemyBullet;
        this.speed = speed;
        this.attackPower = attackPower;
        directionVector += (Vector3)RandomVec;
        this.directionVector = directionVector;
        this.bulletForce = bulletForce;

        float toRotateZ = Vector3.SignedAngle((target+(Vector3)RandomVec)-transform.position, rotationVector, Vector3.forward);
        this.transform.Rotate(new Vector3(0, 0, -1*toRotateZ));
        start = true;
    }

    IEnumerator DeathTimer(float deathTime)
    {
        yield return new WaitForSeconds(deathTime);
        Die();
    }
}
