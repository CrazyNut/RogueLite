using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class MapGeneratorDataScript : MonoBehaviour
{
    public List<MapGeneratorBiome> biomes;
    public Dictionary<string, Pair<int, int>> RandomSegmentsForBiomes;
    public List<MapGeneratorDataObj> genObjects;
    public List<EnemyGenObject> enemies;
    public List<WeaponObject> wearpons;
    public List<Sprite> DifficultySprites;
    private int maxRandom;
    // Start is called before the first frame update
    void Start()
    {
        RandomSegmentsForBiomes = new Dictionary<string, Pair<int, int>>();
        maxRandom = 0;
        foreach (MapGeneratorBiome biome in biomes)
        {
            RandomSegmentsForBiomes.Add(biome.name, new Pair<int, int>(maxRandom, maxRandom + biome.priority));
            maxRandom += biome.priority;
        }
    }

    public string SelectRandomBiome(System.Random rand)
    {
        int random = rand.Next(0, maxRandom);
        foreach (KeyValuePair<string,Pair<int,int>> item in RandomSegmentsForBiomes)
        {
            if(random >= item.Value.First && random < item.Value.Second)
            {
                return item.Key;
            }
        }
        return "";
    }
    public MapGeneratorBiome GetBiomeByName(string name)
    {
        foreach (MapGeneratorBiome item in biomes)
        {
            if (item.name == name)
                return item;
        }
        return null;
    }

    public List<MapGeneratorDataObj> GetObjectsByBiomeName(string name)
    {
        List<MapGeneratorDataObj> objects = new List<MapGeneratorDataObj>();
        foreach (MapGeneratorDataObj item in genObjects)
        {
            if (item.biomesToGen.Contains(name))
                objects.Add(item);
        }
        return objects;
    }

}
