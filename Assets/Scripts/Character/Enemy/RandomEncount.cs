// RandomEncount.cs RandomEncountクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Random Encount")]
public class RandomEncount : ScriptableObject
{
    [System.Serializable]
    public class Data
    {
        [Range(0, 1)] public float EncountRate;
        public EncounterEnemies EncounterEnemies;
    }

    [Range(0, 1)] public float EncountRate = 0.2f;
    public List<Data> List;

    public EncounterEnemies Encount(System.Random rnd)
    {
        if (EncountRate < rnd.NextDouble()) return null;
        foreach(var d in List)
        {
            var t = rnd.NextDouble();
            if(t < d.EncountRate)
            {
                return d.EncounterEnemies;
            }
        }
        return null;
    }
}