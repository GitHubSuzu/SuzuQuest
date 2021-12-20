using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Life Recovery Item")]
public class LifeRecoveryItem : Item
{
    public int RecoveryPower;

    public override void Use(BattleParameterBase target)
    {
        target.HP += RecoveryPower;
        if (target.MaxHP < target.HP) target.HP = target.MaxHP;
    }
}
