using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts
{
    [Serializable]
    public class MapGeneratorBiome
    {
        public string name;
        public TileBase tileToSet;
        public int priority;
        public List<TileBase> MiddleGroundTiles;
        public int maxMiddleGroundTiles;
        public int MaxRoomStage;
        public int Difficulty;
        public Sprite IslandSprite;
        public static MapGeneratorBiome GetBiomeByName(string name, List<MapGeneratorBiome> biomes)
        {
            foreach (MapGeneratorBiome biome in biomes)
            {
                if (biome.name == name)
                    return biome;
            }
            return null;
        }
    }
}
