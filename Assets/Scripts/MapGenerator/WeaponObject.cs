using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class WeaponObject
    {
        public string name;
        public GameObject obj;
        public int weaponLevel;
        public float attackSpeed;
        public float attackPower;
        public float attackRadius;
        public float BOTAttackSpeed;
        public float BOtAttackPower;
        public float BOTAttackRadius;
        public float BOTTimeBeforeAttack;
        public bool IsMelee;
        public GameObject bullet;
        public float bulletSpeed;
        public int bulletQ_ty;
        public Vector2 shootScatter;
        public float bulletDeathTime;
        public bool SplashAttack;

        public static WeaponObject GetWearponByName(string name, List<WeaponObject> wearpons)
        {
            foreach (WeaponObject item in wearpons)
            {
                if (item.name == name)
                    return item;
            }
            return null;
        }
    }
}
