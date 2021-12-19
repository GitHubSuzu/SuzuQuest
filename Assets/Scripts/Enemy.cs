//Enemy.cs Enemyクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Enemy")]
public class Enemy : ScriptableObject
{
    public BattleParameterBase Data;
    public string Name;
    public Sprite Sprite;
    public EnemyAI UseEnemyAI;
    public virtual Enemy Clone()
    {
        var clone = ScriptableObject.CreateInstance<Enemy>();
        clone.Data = new BattleParameterBase();
        Data.CopyTo(clone.Data);
        clone.Name = Name;
        clone.Sprite = Sprite;
        clone.UseEnemyAI = UseEnemyAI.Clone();
        return clone;
    }
    
    public virtual TurnInfo BattleAction(BattleWindow battleWindow)
    {
        return UseEnemyAI.BattleAction(this, battleWindow);
    }
}
