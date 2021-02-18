using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Tilemaps;
using Assets.Scripts;
using TMPro;

public class MapGenerator : MonoBehaviour
{

    public int width;
    public int height;

    public int seed;

    [Range(0, 100)]
    public int randomFillPercent;
    
    public Vector2Int roomCenterPostion;
    int[,] map;

    public GameObject Portal;
    public GameObject Island;
    public TileBase TileToSet;
    public TileBase RoadBorder;
    public GameObject RoadBlocker;
    public Tilemap BackGround;
    public GameObject RoomBG;
    public Tilemap MiddleMap;
    public Tilemap Collision;
    public Tilemap Roads;
    public Transform proceduralParent;
    public GenerationStage genStage;

    public Dictionary<int, Room> rooms;

    public int roomQ_ty;
    public List<Vector2Int> roomPositions;
    public Vector2Int RoomMatrix;
    public int RoomShift;
    public int maxIteration = 10;
    public int maxBridgesForOneIsland = 3;
    public int bridgeRadius = 1;
    private int roomCounter;
    private List<Pair<int, int>> bridges;
    public GameObject tmpObj;
    public Vector2 tmpVec;
    public int bridgeNum = 0;
    public int roomGenStage = 0;
    public int maxRoomGenStage = 0;
    public MapGeneratorDataScript generatorData;
    public float roomRadius = 0.75f;
    public System.Random rand;
    private List<Tilemap> roomTilemaps;


    public float PortalGUIShift;
    void Start()
    {
        roomTilemaps = new List<Tilemap>();
        rand = new System.Random(seed);
        generatorData = this.GetComponent<MapGeneratorDataScript>();
        roomPositions = new List<Vector2Int>();
        for (int i = 0; i < roomQ_ty; i++)
        {
            bool newPos = false;
            int tmp = 0;
            while (newPos == false)
            {
                tmp++;
                roomCenterPostion = new Vector2Int(rand.Next(0, RoomMatrix.x) * width,
                  rand.Next(0, RoomMatrix.y) * height);
                bool roomFound = false;
                foreach (Vector2Int roomPos in roomPositions)
                {
                    if (Mathf.Sqrt(Mathf.Pow(roomCenterPostion.x - roomPos.x, 2) + Mathf.Pow(roomCenterPostion.y - roomPos.y, 2)) < Mathf.Sqrt(width * width + height * height))
                    {
                        roomFound = true;
                        break;
                    }
                }
                if (roomFound == false)
                {
                    roomPositions.Add(roomCenterPostion);
                    newPos = true;
                }
                if (tmp == maxIteration)
                {
                    RoomMatrix.x++;
                    RoomMatrix.y++;
                    tmp = 0;
                }
            }
        }
        roomCounter = 0;
        roomQ_ty = 0;
        bridges = new List<Pair<int, int>>();
        rooms = new Dictionary<int, Room>();


    }
    
    void Update()
    {
        switch (genStage)
        {
            case GenerationStage.StartGeneration:
                if (GenerateMap(roomCounter))
                {
                    roomCounter++;
                    if (roomCounter == roomPositions.Count)
                    {
                        genStage = GenerationStage.DrawRooms;
                        roomCounter = 0;
                    }
                }
                break;
            case GenerationStage.DrawRooms:
                if (DrawMap(roomCounter))
                {
                    roomCounter++;
                }
                if(roomCounter == roomPositions.Count)
                {
                    genStage = GenerationStage.InstPortals;
                }
                break;
            case GenerationStage.InstPortals:
                Transform PortalGUIPos = GameObject.FindGameObjectWithTag("PortalGUIPos").transform;
                PortalGUIController portalsController = GameObject.FindGameObjectWithTag("PortalGUI").GetComponent<PortalGUIController>();
                foreach (KeyValuePair<int, Room> item in rooms)
                {
                    roomPositions[item.Key] = rooms[item.Key].recalcRoomCenter();
                    GameObject obj = Instantiate(tmpObj, BackGround.CellToWorld(new Vector3Int(roomPositions[item.Key].x, roomPositions[item.Key].y, 0)), Quaternion.identity);
                    obj.GetComponent<TextMeshPro>().text = item.Key.ToString();
                    obj.name = item.Key.ToString();

                    Vector2Int portalPos = item.Value.GetRandomPointWithRadius(1, rand);
                    obj = Instantiate(Portal, BackGround.CellToWorld(new Vector3Int(portalPos.x, portalPos.y, 0)), Quaternion.identity);
                    obj.GetComponent<PortalScript>().RoomId = item.Key;
                    obj.name = "Portal room " + item.Key;

                    obj = Instantiate(Island, PortalGUIPos.position, Quaternion.identity);
                    obj.transform.parent = PortalGUIPos;
                    obj.transform.localPosition = new Vector3(item.Value.roomCenter.x / width * PortalGUIShift, item.Value.roomCenter.y / height * PortalGUIShift, 0);
                    obj.GetComponent<SpriteRenderer>().sprite = MapGeneratorBiome.GetBiomeByName(item.Value.biomeName, generatorData.biomes).IslandSprite;
                    portalsController.RegistrateIsland(obj.transform, item.Key);
                }
                genStage = GenerationStage.SetPlayer;
                break;
            case GenerationStage.SetPlayer:

                Vector3Int playerPos = Vector3Int.zero;
                while (true)
                {
                    playerPos.x = rand.Next(0, rooms[0].width) + rooms[0].roomBegin.x;
                    playerPos.y = rand.Next(0, rooms[0].height) + rooms[0].roomBegin.y;
                    if (BackGround.HasTile(playerPos))
                    {
                        break;
                    }
                }
                GameObject.FindGameObjectWithTag("Player").transform.position = BackGround.CellToWorld(playerPos);
                GameObject.FindGameObjectWithTag("Preloader").GetComponent<PreloadScript>().EndPreloader();
                genStage = GenerationStage.none;
                this.GetComponent<WorldScript>().rooms = rooms;
                this.GetComponent<WorldScript>().rand = this.rand;
                this.GetComponent<WorldScript>().InstRoomsClearedDictionary();
                break;
            default:
                break;
        }
       
    }

    static void Swap(ref int e1, ref int e2)
    {
        var temp = e1;
        e1 = e2;
        e2 = temp;
    }
    static void Swap(ref float e1, ref float e2)
    {
        var temp = e1;
        e1 = e2;
        e2 = temp;
    }
    static void BubbleSort(ref int[] array, ref float[] farray)
    {
        var len = array.Length;
        for (var i = 1; i < len; i++)
        {
            for (var j = 0; j < len - i; j++)
            {
                if (farray[j] > farray[j + 1])
                {
                    Swap(ref array[j], ref array[j + 1]);
                    Swap(ref farray[j], ref farray[j + 1]);
                }
            }
        }
        
    }

    void GenerateBridges()
    {
        foreach (KeyValuePair<int,Room> item in rooms)
        {
            roomPositions[item.Key] = rooms[item.Key].recalcRoomCenter();
            GameObject obj = Instantiate(tmpObj, BackGround.CellToWorld(new Vector3Int(roomPositions[item.Key].x, roomPositions[item.Key].y, 0)), Quaternion.identity);
            obj.GetComponent<TextMeshPro>().text = item.Key.ToString();
            obj.name = item.Key.ToString();
        }
        for (int i = 0; i < roomPositions.Count; i++)
        {
            int bridgesToGen = rand.Next(1, maxBridgesForOneIsland);
            int[] array = new int[roomPositions.Count];
            float[] lens = new float[roomPositions.Count];
            for (int j = 0; j < roomPositions.Count; j++)
            {
                array[j] = j;
                lens[j] = Mathf.Sqrt(Mathf.Pow(roomPositions[i].x - roomPositions[j].x, 2) + Mathf.Pow(roomPositions[i].y - roomPositions[j].y, 2));
            }
            BubbleSort(ref array, ref lens);
            int bridgesGenerated = 0;
            for (int j = 0; j < roomPositions.Count; j++)
            {
                if (lens[j] < 0.5f)
                    continue;
                bool canBe = true;
                foreach (Pair<int,int> item in bridges)
                {
                    if (item.First == i && item.Second == array[j] || item.Second == i && item.First == array[j])
                        canBe = false;
                    if(areCrossing(roomPositions[item.First], roomPositions[item.Second], roomPositions[i], roomPositions[array[j]]))
                        canBe = false;
                    if (bridgeOverRoom(i, array[j]))
                        canBe = false;
                }
                if (!canBe)
                {
                    continue;
                }
                else
                {
                    bridges.Add(new Pair<int, int>(i, array[j]));
                    bridgesGenerated++;
                }
                if(bridgesGenerated == bridgesToGen)
                {
                    break;
                }
            }
        }
    }
    bool bridgeOverRoom(int a, int b)
    {
        
        float A = roomPositions[b].y - roomPositions[a].y;
        float B = roomPositions[a].x - roomPositions[b].x;
        float C = roomPositions[b].x * roomPositions[a].y - roomPositions[a].x * roomPositions[b].y;
        float d = 0;
        foreach (KeyValuePair<int, Room> room in rooms)
        {
            if (!room.Value.isBridge && room.Key != a && room.Key != b)
            {
                d = Mathf.Abs(A * room.Value.roomCenter.x + B * room.Value.roomCenter.y + C) / Mathf.Sqrt(A * A + B * B);
                
                if (d < room.Value.width * roomRadius)
                    return true;
            }
        }
        return false;
    }
    private int vector_mult(int ax, int ay, int bx, int by) //векторное произведение
    {
        return ax * by - bx * ay;
    }
    public bool areCrossing(Vector2Int p1, Vector2Int p2, Vector2Int p3, Vector2Int p4)//проверка пересечения
    {
        int v1 = vector_mult(p4.x - p3.x, p4.y - p3.y, p1.x - p3.x, p1.y - p3.y);
        int v2 = vector_mult(p4.x - p3.x, p4.y - p3.y, p2.x - p3.x, p2.y - p3.y);
        int v3 = vector_mult(p2.x - p1.x, p2.y - p1.y, p3.x - p1.x, p3.y - p1.y);
        int v4 = vector_mult(p2.x - p1.x, p2.y - p1.y, p4.x - p1.x, p4.y - p1.y);
        if ((v1 * v2) < 0 && (v3 * v4) < 0)
            return true;
        return false;
    }
  

    bool GenerateMap(int roomNum)
    { 
        
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < 5; i++)
        {
            SmoothMap();
        }
        int counter = 0;
        Vector2Int firstPoint = Vector2Int.zero;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] == 0)
                {
                    counter++;
                    firstPoint.x = i;
                    firstPoint.y = j;
                }
            }
        }

        int tmpCounter = RecalcTmp(firstPoint.x, firstPoint.y);
        if (counter > width * height / 3)
        {
            if (roomNum == 1)
            {
                Debug.Log(tmpCounter + " " + counter);
            }
            string biomeName;
            if (tmpCounter != counter)
            {
                if(tmpCounter > counter - tmpCounter && counter-tmpCounter > counter/5)
                {
                    return false;
                }
                else if(tmpCounter > counter - tmpCounter && counter - tmpCounter <= counter / 5)
                {
                    biomeName = generatorData.SelectRandomBiome(rand);
                    rooms.Add(roomNum, new Room(roomNum, map, height, width, new List<proceduralOBJ>(), new Vector2Int(roomPositions[roomNum].x, roomPositions[roomNum].y), false, biomeName,types.issetTemp));
                    rooms[roomNum].CalcCollision();
                    rooms[roomNum].GenerateObjects(generatorData.GetObjectsByBiomeName(rooms[roomNum].biomeName), rand,this);
                    return true;
                }
                else if(tmpCounter <= counter - tmpCounter && tmpCounter > counter / 5)
                {
                    return false;
                }else if (tmpCounter <= counter - tmpCounter && tmpCounter <= counter / 5)
                {
                    biomeName = generatorData.SelectRandomBiome(rand);
                    rooms.Add(roomNum, new Room(roomNum, map, height, width, new List<proceduralOBJ>(), new Vector2Int(roomPositions[roomNum].x, roomPositions[roomNum].y), false, biomeName, types.isset));
                    rooms[roomNum].CalcCollision();
                    rooms[roomNum].GenerateObjects(generatorData.GetObjectsByBiomeName(rooms[roomNum].biomeName), rand,this);
                    return true;
                }
            }
            biomeName = generatorData.SelectRandomBiome(rand);    
            rooms.Add(roomNum, new Room(roomNum, map, height, width, new List<proceduralOBJ>(), new Vector2Int(roomPositions[roomNum].x, roomPositions[roomNum].y), false, biomeName, types.issetTemp));
            rooms[roomNum].CalcCollision();
            rooms[roomNum].GenerateObjects(generatorData.GetObjectsByBiomeName(rooms[roomNum].biomeName), rand,this);
            return true;
        }
        return false;
    }
    

    public int RecalcTmp(int x,int y)
    {
        int res = 1;
        map[x, y] = (int)types.issetTemp;
        if(GetMapAt(x+1, y) == types.isset)
            res += RecalcTmp(x + 1, y);
        if(GetMapAt(x-1,y) == types.isset)
            res += RecalcTmp(x - 1, y);
        if (GetMapAt(x, y + 1) == types.isset)
            res += RecalcTmp(x, y + 1);
        if (GetMapAt(x, y - 1) == types.isset)
            res += RecalcTmp(x, y - 1);
        return res;
    }

    

    public types GetMapAt(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
            return types.none;
        return (types)map[x, y];
    }

    bool DrawMap(int roomNum)
    {
        for (int i = 0; i < rooms[roomNum].width; i++)
        {
            for (int j = 0; j < rooms[roomNum].height; j++)
            {
                Vector3Int CellPosition = new Vector3Int(i+roomPositions[roomNum].x, j+ roomPositions[roomNum].y, 0);
                types spTipe = (types)rooms[roomNum].map[i, j];
                instTile(spTipe, CellPosition, rooms[roomNum]);
               
            }
        }
        return true;
    }

    public void instTile(types tile, Vector3Int position, Room room = null, bool reinst = true)
    {
        if (!reinst && BackGround.HasTile(position))
            return;
        if (!reinst && tile == types.collider && BackGround.HasTile(position))
            return;
        MapGeneratorBiome biome;
        TileBase tmpTile = TileToSet;
        types type = types.isset;
        if (room != null)
        {
            biome = generatorData.GetBiomeByName(room.biomeName);
            tmpTile = biome.tileToSet;
            type = room.typeOfIsset;
        }
        switch (tile)
        {
            case types.isset:
                if(type == types.isset)
                {
                    BackGround.SetTile(position, tmpTile);
                    Collision.SetTile(position, null);
                }
                break;
            case types.none:
                BackGround.SetTile(position, null);
                break;
            case types.collider:
                if(!(BackGround.HasTile(position) || Roads.HasTile(position)))
                    Collision.SetTile(position, TileToSet);
                break;
            case types.road:
                Roads.SetTile(position, TileToSet);
                Collision.SetTile(position, null);
                break;
            case types.issetTemp:
                if (type == types.issetTemp)
                {
                    BackGround.SetTile(position, tmpTile);
                    Collision.SetTile(position, null);
                }
                break;
            case types.roadCollider:
                if (!Roads.HasTile(position))
                {
                    Roads.SetTile(position, RoadBorder);
                    Collision.SetTile(position, tmpTile);
                }
                break;
            default:
                break;
        }
    }
    

    void RandomFillMap()
    {
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }
                else
                {
                    map[x, y] = (rand.Next(0, 100) < randomFillPercent) ? 1 : 0;
                }
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);

                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;

            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }




}