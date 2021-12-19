//EnemyAI.cs EnemyAIクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : ScriptableObject
{
    public int Turn { get; set; }

    public virtual EnemyAI Clone()
    {
        var clone = ScriptableObject.CreateInstance(GetType()) as EnemyAI;
        return clone;
    }

    public virtual void Init()
    {
        Turn = 0;
    }

    public TurnInfo BattleAction(Enemy enemy, BattleWindow battleWindow)
    {
        Turn++;
        var info = new TurnInfo();
        TurnAI(enemy, battleWindow, info);
        return info;
    }

    protected virtual void TurnAI(Enemy enemy, BattleWindow battleWindow, TurnInfo outTurnInfo)
    {

    }
}
