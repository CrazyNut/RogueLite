using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Room
    {
        public int id;
        public int[,] map;
        public int height;
        public int width;
        public bool isBridge;
        public int BridgePointFirst;
        public int BridgePointSecond;
        public List<proceduralOBJ> objects;
        public Vector2Int roomBegin;
        public Vector2Int roomCenter;
        public string biomeName;
        public types typeOfIsset;
        public Room(int id, int[,] map, int height, int width, List<proceduralOBJ> objects, Vector2Int roomBegin, bool isBridge, string biomeName, types typeOfIsset, int BridgePointFirst = 0, int BridgePointSecond = 0)
        {
            this.id = id;
            this.height = height;
            this.width = width;
            this.map = map;
            this.isBridge = isBridge;
            this.objects = objects;
            this.roomBegin = roomBegin;
            this.BridgePointFirst = BridgePointFirst;
            this.BridgePointSecond = BridgePointSecond;
            this.roomCenter = roomBegin;
            this.biomeName = biomeName;
            this.typeOfIsset = typeOfIsset;
        }

        public Vector2Int GetRandomPointWithRadius(int radius, System.Random rand)
        {
            while (true)
            {
                Vector2Int randPoint = new Vector2Int(rand.Next(0, width), rand.Next(0, height));
                if(IsCellNullReal(randPoint.x,randPoint.y) == 1)
                {
                    for (int i = 0; i < radius; i++)
                    {
                        if (!(IsCellNullReal(randPoint.x + i, randPoint.y, typeOfIsset) && IsCellNullReal(randPoint.x - i, randPoint.y, typeOfIsset) &&
                            IsCellNullReal(randPoint.x, randPoint.y + i, typeOfIsset) && IsCellNullReal(randPoint.x, randPoint.y - i, typeOfIsset)))
                            continue;
                    }
                    randPoint.x += this.roomBegin.x;
                    randPoint.y += this.roomBegin.y;
                    return randPoint;
                }
            }
        }

        public Vector2Int recalcRoomCenter()
        {
            int Ydown = 0, Ytop = 0, XLeft = 0, XRight = 0;
            bool found = false;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        found = true;
                        XLeft = x;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            found = false;
            for (int x = width - 1; x >= 0; x--)
            {
                for (int y = 0; y < height; y++)
                {
                    if (map[x, y] == 1)
                    {
                        found = true;
                        XRight = x;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            found = false;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y] == 1)
                    {
                        found = true;
                        Ydown = y;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            found = false;
            for (int y = height - 1; y >= 0; y--)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, y] == 1)
                    {
                        found = true;
                        Ytop = y;
                        break;
                    }
                }
                if (found)
                {
                    break;
                }
            }
            found = false;
            this.roomCenter.x += (XRight - XLeft) / 2 + XLeft;
            this.roomCenter.y += (Ytop - Ydown) / 2 + Ydown;
            return this.roomCenter;

        }
        public void CalcCollision()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (calcNeighbours(i, j) > 0 && calcNeighbours(i, j) <= 6 && map[i,j] == (int)types.none)
                    {
                        map[i, j] = (int)types.collider;
                    }
                }
            }
        }
        private int calcNeighbours(int x, int y)
        {
            int shift = 0;
            if (this.isBridge)
            shift = this.roomBegin.y % 2 == 0 ? 1 : 0;
            int neighbours = 0;
            if ((y+shift) % 2 == 1)
            {
                neighbours += IsCellNullReal(x, y + 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x, y - 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x - 1, y) != 0 ? 1 : 0;

                neighbours += IsCellNullReal(x + 1, y) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x + 1, y - 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x + 1, y + 1) != 0 ? 1 : 0;
            }
            else
            {
                neighbours += IsCellNullReal(x, y - 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x, y + 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x + 1, y) != 0 ? 1 : 0;

                neighbours += IsCellNullReal(x - 1, y) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x - 1, y + 1) != 0 ? 1 : 0;
                neighbours += IsCellNullReal(x - 1, y - 1) != 0 ? 1 : 0;
            }

            return neighbours;
        }

        private int IsCellNullReal(int x, int y)
        {
            if (GetCellVal(x, y) == (int)types.none || GetCellVal(x, y) == (int)types.collider)
                return 0;
            return 1;
        }

        private bool IsCellNullReal(int x, int y, types type)
        {
            if (GetCellVal(x, y) == (int)type)
                return false;
            return true; 
        }

        private int GetCellVal(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height)
                return (int)types.none;
            return map[x, y];
        }

       
        public void SetTile(int x , int y, types type)
        {
            if (x >= 0 && x < width && y >= 0 && y < height)
                map[x, y] = (int)type;
        }

        public static List<Vector2Int> ColliderMaskWithRadOne(int x, int y)
        {
            List<Vector2Int> mask = new List<Vector2Int>();

            if(y%2 == 0)
            {
                mask.Add(new Vector2Int(x - 1, y + 2));
                mask.Add(new Vector2Int(x , y + 2));
                mask.Add(new Vector2Int(x + 1, y + 2));
                mask.Add(new Vector2Int(x + 1, y + 1));
                mask.Add(new Vector2Int(x + 2, y));
                mask.Add(new Vector2Int(x + 1, y - 1));
                mask.Add(new Vector2Int(x + 1, y - 2));
                mask.Add(new Vector2Int(x , y - 2));
                mask.Add(new Vector2Int(x - 1, y - 2));
                mask.Add(new Vector2Int(x - 2, y - 1));
                mask.Add(new Vector2Int(x - 2, y));
                mask.Add(new Vector2Int(x - 2, y + 1));
            }
            else
            {
                mask.Add(new Vector2Int(x - 1, y + 2));
                mask.Add(new Vector2Int(x, y + 2));
                mask.Add(new Vector2Int(x + 1, y + 2));
                mask.Add(new Vector2Int(x - 1, y - 2));
                mask.Add(new Vector2Int(x, y - 2));
                mask.Add(new Vector2Int(x + 1, y - 2));
                mask.Add(new Vector2Int(x + 2, y + 1));
                mask.Add(new Vector2Int(x + 2, y - 1));
                mask.Add(new Vector2Int(x + 2, y));
                mask.Add(new Vector2Int(x - 2, y));
                mask.Add(new Vector2Int(x - 1, y + 1));
                mask.Add(new Vector2Int(x - 1, y - 1));
            }

            return mask;
        }
       

        public static void AddRangeList(ref List<Vector2Int> res, List<Vector2Int> toAdd)
        {
            foreach (Vector2Int toAddItem in toAdd)
            {
                bool isset = false;
                foreach (Vector2Int resItem in res)
                {
                    if(toAddItem.x == resItem.x && toAddItem.y == resItem.y)
                    {
                        isset = true;
                        break;
                    }
                }
                if (!isset)
                {
                    res.Add(toAddItem);
                }
            }
        }

        public void GenerateObjects(List<MapGeneratorDataObj> objectsToGen, System.Random rand, MapGenerator mg)
        {
            objects = new List<proceduralOBJ>();
            foreach (MapGeneratorDataObj item in objectsToGen)
            {
                for (int i = 0; i < rand.Next(item.MinQ_ty,item.MaxQ_ty); i++)
                {
                    bool objGenerated = false;
                    while (!objGenerated)
                    {
                        int x = rand.Next(0, width);
                        int y = rand.Next(0, height);
                        if (map[x, y] == (int)types.none || map[x, y] == (int)types.collider)
                            continue;
                        bool found = false;
                        foreach (proceduralOBJ AlreadyGeneratedObj in objects)
                        {
                            if(AlreadyGeneratedObj.pos.x == x && AlreadyGeneratedObj.pos.y == y)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            GameObject.Instantiate(item.obj, mg.BackGround.CellToWorld(new Vector3Int(x + this.roomBegin.x, y + this.roomBegin.y, 0)), Quaternion.identity).transform.parent = mg.proceduralParent;

                            objGenerated = true;
                        }
                    }
                }
            }
        }
    }
}
