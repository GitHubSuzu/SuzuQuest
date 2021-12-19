//BattleParameter.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    public bool IsNowDefense { get; set; } = false;
    public bool IsLimitItemCount { get => Items.Count >= 4; }
    
    [Min(1)] public int LimitAttack = 100;
    [Min(1)] public int LimitDefense = 100;
    [Min(1)] public int LimitHP = 500;
 
    public void AdjustParamWithLevel()
    {
        Level = Exp / 100;
        Attack = (int)(LimitAttack * Level / 100f);
        Defense = (int)(LimitDefense * Level / 100f);
        MaxHP = (int)(LimitHP * Level / 100f);
    }

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
        
        dest.LimitAttack = LimitAttack;
        dest.LimitDefense = LimitDefense;
        dest.LimitHP = LimitHP;
    }
    
    public bool GetExp(int exp)
    {
        Exp += exp;
        
        if(Exp >= (Level+1) * 100)
        {
            AdjustParamWithLevel();
            return true;
        }
        
        return false;
    }
    public class AttackResult
    {
        public int LeaveHP;
        public int Damage;
    }
    public virtual bool AttackTo(BattleParameterBase target, out AttackResult result)
    {
        result = new AttackResult();

        result.Damage = Mathf.Max(0, AttackPower - target.DefensePower);
        if (target.IsNowDefense)
        {
            result.Damage /= 2;
        }
        target.HP -= result.Damage;
        result.LeaveHP = target.HP;
        return target.HP <= 0;
    }
}

[CreateAssetMenu(menuName = "BattleParameter")]
public class BattleParameter : ScriptableObject
{
    public BattleParameterBase Data;
}
