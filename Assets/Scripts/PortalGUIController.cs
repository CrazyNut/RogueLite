using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalGUIController : MonoBehaviour
{
    public JoyButton exit;
    public JoyButton Attack;
    public Joystick controller;
    public float speed;
    private Animator animator;
    private MainCameraScript cameraScript;
    public Vector3 moveVelocity;
    public List<Transform> Islands;
    public Vector3 PortalGUITopRightPoint;
    public Vector3 PortalGUIDownLeftPoint;
    public float shift;
    public int IslandId;
    public WorldScript ws;
    public int GoingToattackRoomId;
    public Vector3 PortalTarget;
    public int PortalTargetId;
    private void Start()
    {
        PortalTargetId = -1;
        GoingToattackRoomId = -1;
        ws = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldScript>();
        Islands = new List<Transform>();
        PortalGUITopRightPoint = GameObject.FindGameObjectWithTag("PortalGUIPos").transform.position;
        PortalGUIDownLeftPoint = GameObject.FindGameObjectWithTag("PortalGUIPos").transform.position;
        animator = this.GetComponent<Animator>();
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCameraScript>();
    }

    private void Update()
    {
        if (exit.Pressed)
        {
            DisablePortalGUI();
        }

        if (Attack.Pressed && GoingToattackRoomId != -1)
        {
            PortalTargetId = GoingToattackRoomId;
            DisablePortalGUI();
        }

        moveVelocity = new Vector2(controller.Horizontal, controller.Vertical);


        if (cameraScript.transform.position.x >= PortalGUITopRightPoint.x + shift && moveVelocity.x > 0)
            moveVelocity.x = 0;
        if (cameraScript.transform.position.y >= PortalGUITopRightPoint.y + shift && moveVelocity.y > 0)
            moveVelocity.y = 0;
        if (cameraScript.transform.position.x <= PortalGUIDownLeftPoint.x - shift && moveVelocity.x < 0)
            moveVelocity.x = 0;
        if (cameraScript.transform.position.y <= PortalGUIDownLeftPoint.y - shift && moveVelocity.y < 0)
            moveVelocity.y = 0;


        cameraScript.transform.position += moveVelocity * speed * Time.deltaTime;

        
        RaycastHit hit;
        GameObject HitOBj = null;
        if (Physics.Raycast(cameraScript.transform.position, cameraScript.transform.TransformDirection(Vector3.forward), out hit))
        {
            if(hit.transform.tag == "Island")
            {
                HitOBj = hit.transform.gameObject;
                if (HitOBj.GetComponent<IslandController>().RoomId != GoingToattackRoomId && GoingToattackRoomId != -1)
                {
                    if (ws.roomsAreCleared[GoingToattackRoomId])
                        HitOBj.GetComponent<IslandController>().SetCleared(true);
                    else
                        HitOBj.GetComponent<IslandController>().SetGoingToAttack(false);
                }
                GoingToattackRoomId = HitOBj.GetComponent<IslandController>().RoomId;
                if (IslandId != GoingToattackRoomId)
                {
                    HitOBj.GetComponent<IslandController>().SetGoingToAttack(true);
                }
            }
            else if (HitOBj != null)
            {

                if (IslandId != GoingToattackRoomId && GoingToattackRoomId != -1)
                {
                    if (ws.roomsAreCleared[GoingToattackRoomId])
                        HitOBj.GetComponent<IslandController>().SetCleared(true);
                    else
                        HitOBj.GetComponent<IslandController>().SetGoingToAttack(false);
                }
                GoingToattackRoomId = -1;
                HitOBj = null;
            }
        }
        else
        {
            if (IslandId != GoingToattackRoomId && GoingToattackRoomId != -1)
            {
                if (ws.roomsAreCleared[GoingToattackRoomId])
                    Islands[GoingToattackRoomId].GetComponent<IslandController>().SetCleared(true);
                else
                    Islands[GoingToattackRoomId].GetComponent<IslandController>().SetGoingToAttack(false);
            }
            GoingToattackRoomId = -1;
            HitOBj = null;
        }
    }

    public void RegistrateIsland(Transform island, int RoomId)
    {
        island.GetComponent<IslandController>().RoomId = RoomId;
        Islands.Add(island);
        if (island.position.x > PortalGUITopRightPoint.x)
            PortalGUITopRightPoint.x = island.position.x;
        if (island.position.x < PortalGUIDownLeftPoint.x)
            PortalGUIDownLeftPoint.x = island.position.x;
        if (island.position.y > PortalGUITopRightPoint.y)
            PortalGUITopRightPoint.y = island.position.y;
        if (island.position.y < PortalGUIDownLeftPoint.y)
            PortalGUIDownLeftPoint.y = island.position.y;
    }

    public void TeleportPlayer()
    {
        if(PortalTargetId != IslandId)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PersonController>().UsersPlayer)
                {
                    PortalTarget = ws.portals[PortalTargetId].transform.position;
                    player.GetComponent<PersonController>().teleport = true;
                    player.GetComponent<PersonController>().PortalTarget = PortalTarget;
                    player.GetComponent<PersonController>().PortalTargetId = PortalTargetId;
                }
            }
        }
        
    }

    public void EnablePortalGUI(int RoomId)
    {
        this.GetComponent<Canvas>().enabled = true;
        this.animator.SetBool("Enable",true);
        IslandId = RoomId;
        Islands[IslandId].GetComponent<IslandController>().SetNowHere(true);
    }

    public void DisablePortalGUI()
    {

        Islands[IslandId].GetComponent<IslandController>().SetNowHere(false);
        for (int i = 0; i < ws.roomsAreCleared.Count; i++)
        {
            if (i != IslandId)
            {
                if (ws.roomsAreCleared[i])
                {

                    Islands[IslandId].GetComponent<IslandController>().SetCleared(true);
                }
                else
                {
                    Islands[IslandId].GetComponent<IslandController>().SetCleared(false);
                }
            }
        }
        this.animator.SetBool("Enable", false);
    }

    public void TransformCameraToIslandsGUI()
    {
        GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<MeshRenderer>().enabled = false;
        GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<Canvas>().enabled = false;
        Vector3 pos = GameObject.FindGameObjectWithTag("PortalGUIPos").transform.position;
        pos.z = -1;
        cameraScript.transform.position = pos;
    }

    public void ReturnCameraTOPlayer()
    {
        GameObject.FindGameObjectWithTag("PlayerHealthText").GetComponent<MeshRenderer>().enabled = true;
        GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<Canvas>().enabled = true;
        cameraScript.showPortals = false;
    }
}
