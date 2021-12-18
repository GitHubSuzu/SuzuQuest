//Player.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

public class Player : CharacterBase
{
    protected override void Start()
    {
        DoMoveCamera = true;
        base.Start();
    }
}