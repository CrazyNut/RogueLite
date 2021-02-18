using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public struct MapGeneratorDataObj
    {
        public string name;
        public GameObject obj;
        public int MinQ_ty;
        public int MaxQ_ty;
        public List<string> biomesToGen;
    }
}
