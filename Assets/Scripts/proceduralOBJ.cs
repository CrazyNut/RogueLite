using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class proceduralOBJ
    {
        public Vector2 pos;
        public string name;
        public GameObject obj;
        public proceduralOBJ(Vector2 pos, string name, GameObject obj)
        {
            this.pos = pos;
            this.name = name;
            this.obj = obj;
        }
    }
}
