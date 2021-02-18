using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.Scripts;

public class WorldScript : MonoBehaviour
{
    public Dictionary<int, Room> rooms;
    public Dictionary<int, bool> roomsAreCleared;
    public Dictionary<int, PortalScript> portals;
    public MapGeneratorDataScript data;
    public System.Random rand;
    public int nowWave = 0;
    public Vector2Int minMaxWave;
    public MapGenerator mg;
    public int DiffLevel = 0;
    public int RoomID;
    public int RoomStage = 0;    
    public int MaxRoomStage;
    private int enemiesInRoom = 0;
    public int EnemiesInRoom
    {
        get
        {
            return enemiesInRoom;
        }
        set
        {
            enemiesInRoom = value;
            if(value <= 0)
            {
                if(MaxRoomStage == RoomStage)
                {
                    RoomStage = 0;
                    OpenRoom(RoomID);
                    roomsAreCleared[RoomID] = true;
                }
                else
                {
                    RoomStage++;
                    InstEnemiesInRoom(RoomID);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        data = this.GetComponent<MapGeneratorDataScript>();
        portals = new Dictionary<int,PortalScript>();
        roomsAreCleared = new Dictionary<int, bool>();
        mg = this.GetComponent<MapGenerator>();
    }
    
    

    public void InstRoomsClearedDictionary()
    {
        foreach (KeyValuePair<int,Room> keyValuePair in rooms)
        {
            if (!keyValuePair.Value.isBridge)
            {
                roomsAreCleared.Add(keyValuePair.Key, false);
            }
        }

        CloseRoom(0);
        InstEnemiesInRoom(0);
        MaxRoomStage = MapGeneratorBiome.GetBiomeByName(rooms[0].biomeName, data.biomes).MaxRoomStage;
        RoomID = 0;
    }


    public void Teleported(int RoomNum)
    {
        CheckIfStartBattle(portals[RoomNum]);
    }


    public void RegistratePortal(PortalScript portal)
    {
        portals.Add(portal.RoomId, portal);
    }

    public void CheckIfStartBattle(PortalScript blocker)
    {
        if (!roomsAreCleared[blocker.RoomId])
        {
            int playerIndex = 0;
            Transform MainPlayer = null;
            float dist = 1000000f;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (DistanseBetween(player.transform.position, new Vector3( rooms[blocker.RoomId].roomCenter.x, rooms[blocker.RoomId].roomCenter.y, 0)) < 
                    dist)
                {
                    dist = DistanseBetween(player.transform.position, new Vector3(rooms[blocker.RoomId].roomCenter.x, rooms[blocker.RoomId].roomCenter.y, 0));
                    MainPlayer = player.transform;
                    break;
                }
                playerIndex++;
            }
            int t = 0;
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (t == playerIndex)
                    continue;
                player.transform.position = MainPlayer.position;
                t++;
            }
            CloseRoom(blocker.RoomId);
            this.MaxRoomStage = MapGeneratorBiome.GetBiomeByName(rooms[blocker.RoomId].biomeName,data.biomes).MaxRoomStage;
            InstEnemiesInRoom(blocker.RoomId);
            this.RoomID = blocker.RoomId;
        }
    }

    private void CloseRoom(int RoomId)
    {
        portals[RoomID].portalEnabled = false;
    }

    private void OpenRoom(int RoomId)
    {

        portals[RoomID].portalEnabled = true;
    }

    public float DistanseBetween(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }
    

    public void InstEnemiesInRoom(int RoomNum)
    {
        Room room = rooms[RoomNum];
        List<EnemyGenObject> enemies = EnemyGenObject.EnemiesINBiome(rooms[RoomNum].biomeName, DiffLevel ,data.enemies);
        foreach (EnemyGenObject enemy in enemies)
        {
            for (int i = 0; i < rand.Next(enemy.MinQ_ty,enemy.MaxQ_ty); i++)
            {
                bool enemyGenerated = false;
                while (!enemyGenerated)
                {
                    int x = rand.Next(0, room.width) + room.roomBegin.x;
                    int y = rand.Next(0, room.height) + room.roomBegin.y;
                    if (!mg.BackGround.HasTile(new Vector3Int(x,y,0)))
                        continue;
                    else
                    {
                        EnemyScript enObj = GameObject.Instantiate(enemy.obj, mg.BackGround.CellToWorld(new Vector3Int(x, y, 0)), Quaternion.identity).GetComponent<EnemyScript>();
                        Debug.Log(enObj.transform.position);
                        enObj.weapon = WeaponObject.GetWearponByName(enemy.weapon, data.wearpons);
                        GameObject weapon = GameObject.Instantiate(enObj.weapon.obj, enObj.WeaponPosition.transform.position, enObj.weapon.obj.transform.rotation);
                        weapon.transform.parent = enObj.WeaponPosition.transform;
                        weapon.GetComponent<weaponScript>().weaponData = enObj.weapon;
                        weapon.GetComponent<weaponScript>().EnemyWeapon = true;
                        enObj.weaponScript = weapon.GetComponent<weaponScript>();
                        this.EnemiesInRoom++;
                        break;
                    }
                }
            }
        }
    }

}
