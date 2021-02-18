using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class RoomHelper
    {
        public static List<Vector2Int> makeLine(Vector2Int begin, Vector2Int end)
        {
            List<Vector2Int> res = new List<Vector2Int>();
            res.Add(begin);
            while (true)
            {
                Directions bestEndDir = Directions.DownLeft;
                Vector2Int bestTargetPoint = DirToCoord(begin.x, begin.y, bestEndDir);
                float bestLen = CalcLen(bestTargetPoint, end);
                foreach (Directions endDir in (Directions[])Enum.GetValues(typeof(Directions)))
                {
                    Vector2Int targetPoint = DirToCoord(begin.x, begin.y, endDir);
                    float len = CalcLen(targetPoint, end);
                    if (len < bestLen)
                    {
                        bestEndDir = endDir;
                        bestTargetPoint = targetPoint;
                        bestLen = len;
                    }
                }
                res.Add(bestTargetPoint);
                begin = bestTargetPoint;
                if (CalcLen(bestTargetPoint, end) < 1.0f)
                    break;
            }

            return res;
        }
      

        public static List<Vector2Int> makeLineNew(Vector2Int begin, Vector2Int end)
        {
            List<Vector2Int> res = new List<Vector2Int>();

            if(begin.x == end.x)
            {
                if(begin.y < end.y)
                {
                    for (int y = begin.y; y < end.y; y++)
                    {
                        res.Add(new Vector2Int(begin.x - 1, y));
                        res.Add(new Vector2Int(begin.x, y));
                        res.Add(new Vector2Int(begin.x + 1, y));
                    }
                }
                else
                {
                    for (int y = begin.y-1; y >= end.y; y--)
                    {
                        res.Add(new Vector2Int(begin.x - 1, y));
                        res.Add(new Vector2Int(begin.x, y));
                        res.Add(new Vector2Int(begin.x + 1, y));
                    }
                }
            }
            else if(begin.y == end.y)
            {
                if(begin.x < end.x)
                {
                    for (int x = begin.x; x < end.x; x++)
                    {
                        res.Add(new Vector2Int(x, begin.y + 1));
                        res.Add(new Vector2Int(x, begin.y));
                        res.Add(new Vector2Int(x, begin.y - 1));
                    }
                }
                else
                {
                    for (int x = begin.x - 1; x >= end.x; x--)
                    {
                        res.Add(new Vector2Int(x, begin.y + 1));
                        res.Add(new Vector2Int(x, begin.y));
                        res.Add(new Vector2Int(x, begin.y - 1));
                    }
                }
            }
            else if(begin.x < end.x)
            {
                if(begin.y < end.y)
                {
                    for (int x = begin.x; x < end.x + 2; x++)
                    {
                        res.Add(new Vector2Int(x, begin.y + 1));
                        res.Add(new Vector2Int(x, begin.y));
                        res.Add(new Vector2Int(x, begin.y - 1));
                    }
                    begin.x = end.x;
                    for (int y = begin.y; y < end.y; y++)
                    {
                        res.Add(new Vector2Int(begin.x - 1, y));
                        res.Add(new Vector2Int(begin.x, y));
                        res.Add(new Vector2Int(begin.x + 1, y));
                    }
                }
            }
            

            return res;
        }


     

        public static List<Vector2Int> DrawRadMask(int x, int y)
        {
            List<Vector2Int> mask = new List<Vector2Int>();
            mask.Add(new Vector2Int(x, y));
            if (y % 2 == 0)
            {
                mask.Add(new Vector2Int(x, y));
                mask.Add(new Vector2Int(x, y + 1));
                mask.Add(new Vector2Int(x, y - 1));
                mask.Add(new Vector2Int(x + 1, y));
                mask.Add(new Vector2Int(x - 1, y + 1));
                mask.Add(new Vector2Int(x - 1, y));
                mask.Add(new Vector2Int(x - 1, y - 1));
                
            }
            else
            {
                mask.Add(new Vector2Int(x, y));
                mask.Add(new Vector2Int(x, y + 1));
                mask.Add(new Vector2Int(x, y - 1));
                mask.Add(new Vector2Int(x - 1, y));
                mask.Add(new Vector2Int(x + 1, y + 1));
                mask.Add(new Vector2Int(x + 1, y));
                mask.Add(new Vector2Int(x + 1, y - 1));
            }
            return mask;
        }
        public static float CalcLen(Vector2Int begin, Vector2 end)
        {
            return Mathf.Sqrt(Mathf.Pow(begin.x - end.x, 2) + Mathf.Pow(begin.y - end.y, 2));
        }

        public static Vector2Int DirToCoord(int x, int y, Directions dir)
        {
            Pair<int, int> resPair = new Pair<int, int>(x, y);
            switch (dir)
            {
                case Directions.TopLeft:
                    if (y % 2 == 1)
                    {
                        resPair.Second++;
                    }
                    else
                    {
                        resPair.Second++;
                        resPair.First--;
                    }
                    break;
                case Directions.TopRight:
                    if(y%2 == 1)
                    {
                        resPair.Second++;
                        resPair.First++;
                    }
                    else
                    {
                        resPair.Second++;
                    }
                    break;
                case Directions.Left:
                    resPair.First--;
                    break;
                case Directions.Right:
                    resPair.First++;
                    break;
                case Directions.DownLeft:
                    if (y % 2 == 1)
                    {
                        resPair.Second--;
                    }
                    else
                    {
                        resPair.Second--;
                        resPair.First--;
                    }
                    break;
                case Directions.DownRight:
                    if(y%2 == 1)
                    {
                        resPair.Second--;
                        resPair.First++;
                    }
                    else
                    {
                        resPair.Second--;
                    }
                    break;
                default:
                    break;
            }
            return new Vector2Int(resPair.First, resPair.Second);
        }

        public static bool RoomsContains(Vector2Int vec, Dictionary<int,Room> rooms)
        {
            bool found = false;
            foreach (KeyValuePair<int,Room> item in rooms)
            {
                if(vec.x >= item.Value.roomCenter.x && vec.x < item.Value.roomCenter.x + item.Value.width && vec.y >= item.Value.roomCenter.y && vec.y < item.Value.roomCenter.y + item.Value.height)
                {
                    Vector2Int tmp = new Vector2Int(vec.x - item.Value.roomCenter.x, vec.y - item.Value.roomCenter.y);
                    //if(item)
                }
            }
            return found;
        }
        
    }
}
