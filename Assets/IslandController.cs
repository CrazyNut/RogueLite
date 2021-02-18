using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandController : MonoBehaviour
{
    public Animator anim;
    public int RoomId;
    public bool Cleared = false;
    public bool GoingToAttack = false;
    public bool nowHere = false;
    // Start is called before the first frame update
    void Start()
    {
        anim = transform.Find("Pointer").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNowHere(bool val)
    {
        Cleared = false;
        GoingToAttack = false;
        nowHere = val;
        anim.SetBool("Cleared", false);
        anim.SetBool("GoingToAttack", false);
        anim.SetBool("NowHere", val);
    }
    public void SetCleared(bool val)
    {
        Cleared = val;
        GoingToAttack = false;
        nowHere = false;
        anim.SetBool("Cleared", val);
        anim.SetBool("GoingToAttack", false);
        anim.SetBool("NowHere", false);
    }
    public void SetGoingToAttack(bool val)
    {
        Debug.Log("Island " + RoomId + " " + val.ToString());
        Cleared = false;
        GoingToAttack = val;
        nowHere = false;
        anim.SetBool("Cleared", false);
        anim.SetBool("GoingToAttack", val);
        anim.SetBool("NowHere", false);
    }

}
