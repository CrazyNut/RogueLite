using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{
    [Serializable]
    public class EnemyGenObject
    {
        public string name;
        public GameObject obj;
        public string weapon;
        public int WhatDiffLevelFromToGen;
        public bool IsBoss = false;
        public int MinQ_ty;
        public int MaxQ_ty;
        public List<string> BiomesToGen;

        public static List<EnemyGenObject> EnemiesINBiome(string BiomeName, int diff, List<EnemyGenObject> allEnemies)
        {
            List<EnemyGenObject> res = new List<EnemyGenObject>();
            foreach (EnemyGenObject item in allEnemies)
            {
                if (item.BiomesToGen.Contains(BiomeName) && item.WhatDiffLevelFromToGen == diff)
                    res.Add(item);
            }
            return res;
        }
    }
}
