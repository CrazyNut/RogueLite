using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public int RoomId;
    public bool portalEnabled = false;
    public bool showIslands = false;
    public WorldScript ws;
    public PersonController player;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (item.GetComponent<PersonController>().UsersPlayer)
                player = item.GetComponent<PersonController>();
        }
        ws = GameObject.FindGameObjectWithTag("Map").GetComponent<WorldScript>();
        ws.RegistratePortal(this);
    }

    // Update is called once per frame
    void Update()
    {
        if((Input.GetKey(KeyCode.P) || player.AttackButton.Pressed) && portalEnabled && showIslands)
        {
            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<MainCameraScript>().ShowPortalGUI(RoomId);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            showIslands = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            showIslands = false;
        }
    }
}
