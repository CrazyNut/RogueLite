using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour
{
    private Transform player;
    public bool showPortals;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PersonController>().UsersPlayer)
                this.player = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!showPortals)
        {
            this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        }
    }

    public void ShowPortalGUI(int RoomId)
    {
        if (!showPortals)
        {
            showPortals = true;
            GameObject.FindGameObjectWithTag("PortalGUI").GetComponent<PortalGUIController>().EnablePortalGUI(RoomId);
        }
    }
}
