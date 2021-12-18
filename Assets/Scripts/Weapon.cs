//Weapon.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponKind
{
    Attack,
    Defense,
}

[CreateAssetMenu(menuName = "Item/Weapon")]
public class Weapon : Item
{
    public WeaponKind Kind;
    public int Power;
}
