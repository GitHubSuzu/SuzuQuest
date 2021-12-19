//EncounterEnemies.cs 戦闘中の敵データを表すもの
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EncounterEnemy")]
public class EncounterEnemies : ScriptableObject
{
    [Range(0, 1)] public float EscapeSuccessRate = 0.7f;
    public List<Enemy> Enemies;

    public EncounterEnemies Clone()
    {
        var clone = ScriptableObject.CreateInstance<EncounterEnemies>();
        clone.EscapeSuccessRate = EscapeSuccessRate;
        clone.Enemies = new List<Enemy>(Enemies.Count);
        foreach(var e in Enemies)
        {
            clone.Enemies.Add(e.Clone());
        }
        return clone;
    }
}