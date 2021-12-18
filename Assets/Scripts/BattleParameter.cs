//BattleParameter.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BattleParameterBase
{
    [Min(1)] public int HP;
    [Min(1)] public int MaxHP;

    [Min(1)] public int Attack;
    [Min(1)] public int Defense;

    [Min(1)] public int Level;
    [Min(0)] public int Exp;
    [Min(0)] public int Money;

    public Weapon AttackWeapon;
    public Weapon DefenseWeapon;

    public List<Item> Items; //上限4個までを想定して他のものを作成している。

    public int AttackPower { get => Attack + (AttackWeapon != null ? AttackWeapon.Power : 0); }
    public int DefensePower { get => Defense + (DefenseWeapon != null ? DefenseWeapon.Power : 0); }

    public bool IsLimitItemCount { get => Items.Count >= 4; }
    public virtual void CopyTo(BattleParameterBase dest)
    {
        dest.HP = HP;
        dest.MaxHP = HP < MaxHP ? MaxHP : HP;
        dest.Attack = Attack;
        dest.Defense = Defense;
        dest.Level = Level;
        dest.Exp = Exp;
        dest.Money = Money;

        dest.AttackWeapon = AttackWeapon;
        dest.DefenseWeapon = DefenseWeapon;

        dest.Items = new List<Item>(Items.ToArray());
    }
}

[CreateAssetMenu(menuName = "BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}
