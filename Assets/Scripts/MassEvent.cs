//MassEvent.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MassEvent : ScriptableObject
{
    public TileBase Tile;

    public virtual void Exec(RPGSceneManager manager) {}
}
