using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
public class weaponScript : MonoBehaviour
{
    public bool attack;
    public bool lastAttackState;
    public bool canAttack = true;
    public PersonController player;
    public EnemyScript enemy;
    public bool EnemyWeapon = false;
    public WeaponObject weaponData;
    public float weaponForce;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null)
        {
            float toRotateZ;
            if (player.transform.position.x < transform.position.x)
            {
                toRotateZ = Vector3.SignedAngle(player.transform.position - transform.position, Vector3.left, Vector3.forward);
            }
            else
            {
                toRotateZ = -1 * Vector3.SignedAngle(player.transform.position - transform.position, Vector3.right, Vector3.forward);
            }
            transform.parent.Rotate(new Vector3(0,0, toRotateZ - transform.parent.localEulerAngles.z));
        }
        else if (enemy != null)
        {
            float toRotateZ;
            if (enemy.transform.position.x < transform.position.x)
            {
                toRotateZ = Vector3.SignedAngle(enemy.transform.position - transform.position, Vector3.left, Vector3.forward);
            }
            else
            {
                toRotateZ = -1 * Vector3.SignedAngle(enemy.transform.position - transform.position, Vector3.right, Vector3.forward);
            }
            transform.parent.Rotate(new Vector3(0, 0, toRotateZ - transform.parent.localEulerAngles.z));
        }
        else
        {
            transform.parent.Rotate(transform.parent.localEulerAngles * -1);
        }
    }

    public void AttackEnemies(List<EnemyScript> enemies, Vector3 Direction)
    {
        if (this.canAttack)
        {
            this.canAttack = false;
            if (enemies.Count != 0)
            {
                this.enemy = enemies[0];
            }
            else
            {
                this.enemy = null;
            }
            this.attack = true;
            this.GetComponent<Animator>().SetBool("Attack", true);
            if (this.player == null && this.enemy == null)
                return;
            if (weaponData.IsMelee)
            {
                if (EnemyWeapon)
                {
                    Debug.Log(Direction * weaponForce);
                    player.AddForce(Direction * weaponForce);
                    player.Health -= weaponData.BOtAttackPower;
                }
                else
                {
                    if (enemy != null)
                    {
                        foreach (EnemyScript enem in enemies)
                        {
                            enem.AddForce(Direction * weaponForce);
                            enem.Health -= weaponData.attackPower;
                        }
                    }
                }
            }
            else
            {
                if (EnemyWeapon)
                {
                    for (int i = 0; i < weaponData.bulletQ_ty; i++)
                    {
                        GameObject.Instantiate(weaponData.bullet, transform.position, Quaternion.identity).GetComponent<BulletScript>()
                            .StartBullet(true, player.transform.position, Direction, weaponData.bulletSpeed, weaponData.bulletDeathTime, weaponData.BOtAttackPower,
                            new Vector2(UnityEngine.Random.Range(-1 * weaponData.shootScatter.x, weaponData.shootScatter.x), UnityEngine.Random.Range(-1 * weaponData.shootScatter.y, weaponData.shootScatter.y)), weaponForce);
                    }
                }
                else
                {
                    for(int i = 0; i < weaponData.bulletQ_ty; i++)
                    {
                        GameObject.Instantiate(weaponData.bullet, transform.position, Quaternion.identity).GetComponent<BulletScript>()
                            .StartBullet(false, player.transform.position, Direction, weaponData.bulletSpeed, weaponData.bulletDeathTime, weaponData.attackPower,
                            new Vector2(UnityEngine.Random.Range(-1 * weaponData.shootScatter.x, weaponData.shootScatter.x), UnityEngine.Random.Range(-1 * weaponData.shootScatter.y, weaponData.shootScatter.y)), weaponForce);
                     }
                }
            }
        }
    }

    public void Attack(PersonController player, EnemyScript enemy, Vector3 Direction)
    {
        if (this.canAttack)
        {
            this.canAttack = false;
            this.player = player;
            this.enemy = enemy;
            this.attack = true;
            this.GetComponent<Animator>().SetBool("Attack", true);
            if (weaponData.IsMelee)
            {
                if (EnemyWeapon)
                {
                    if(player != null)
                    {
                        Debug.Log(Direction * weaponForce);
                        player.AddForce(Direction * weaponForce);
                        player.Health -= weaponData.BOtAttackPower;
                    }
                }
                else
                {
                    if(enemy != null)
                    {
                        enemy.AddForce(Direction * weaponForce);
                        enemy.Health -= weaponData.attackPower;
                    }
                }
            }
            else
            {
                if (EnemyWeapon)
                {
                    for (int i = 0; i < weaponData.bulletQ_ty; i++)
                    {
                        GameObject.Instantiate(weaponData.bullet, transform.position, Quaternion.identity).GetComponent<BulletScript>()
                            .StartBullet(true, player.transform.position, Direction, weaponData.bulletSpeed, weaponData.bulletDeathTime, weaponData.BOtAttackPower,
                            new Vector2(UnityEngine.Random.Range(-1 * weaponData.shootScatter.x, weaponData.shootScatter.x), UnityEngine.Random.Range(-1 * weaponData.shootScatter.y, weaponData.shootScatter.y)), weaponForce);
                    }
                }
                else
                {
                    for (int i = 0; i < weaponData.bulletQ_ty; i++)
                    {
                        if(enemy == null)
                        {
                            GameObject.Instantiate(weaponData.bullet, transform.position, Quaternion.identity).GetComponent<BulletScript>()
                            .StartBullet(false, Direction * 10, Direction, weaponData.bulletSpeed, weaponData.bulletDeathTime, weaponData.attackPower,
                            new Vector2(UnityEngine.Random.Range(-1 * weaponData.shootScatter.x, weaponData.shootScatter.x), UnityEngine.Random.Range(-1 * weaponData.shootScatter.y, weaponData.shootScatter.y)), weaponForce);
                        }
                        else
                        {
                            GameObject.Instantiate(weaponData.bullet, transform.position, Quaternion.identity).GetComponent<BulletScript>()
                            .StartBullet(false, enemy.transform.position, Direction, weaponData.bulletSpeed, weaponData.bulletDeathTime, weaponData.attackPower,
                            new Vector2(UnityEngine.Random.Range(-1 * weaponData.shootScatter.x, weaponData.shootScatter.x), UnityEngine.Random.Range(-1 * weaponData.shootScatter.y, weaponData.shootScatter.y)), weaponForce);
                        }
                        
                    }
                }
            }
        }
    }


    public void EndAttack()
    {
        this.attack = false;
        this.GetComponent<Animator>().SetBool("Attack", false);
        StartCoroutine(UpdateAttackTimer());
    }


    IEnumerator UpdateAttackTimer()
    {
        yield return new WaitForSeconds(weaponData.BOTAttackSpeed);
        canAttack = true;
    }
}
