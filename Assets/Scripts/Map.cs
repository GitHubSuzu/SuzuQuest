//Map.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Grid Grid { get => GetComponent<Grid>(); }
    Dictionary<string, Tilemap> _tilemaps;

    readonly static string BACKGROND_TILEMAP_NAME = "BackGround";
    readonly static string NONE_OBJECTS_TILEMAP_NAME = "NoneObjects";
    readonly static string OBJECTS_TILEMAP_NAME = "Objects";
    readonly static string EVENT_BOX_TILEMAP_NAME = "EventBox";

    private void Awake()
    {
        _tilemaps = new Dictionary<string, Tilemap>();
        foreach (var tilemap in Grid.GetComponentsInChildren<Tilemap>())
        {
            _tilemaps.Add(tilemap.name, tilemap);
        }
    }

    public Vector3 GetWorldPos(Vector3Int pos)
    {
        return Grid.CellToWorld(pos);
    }

    public class Mass
    {
        public bool isMovable;
        public TileBase eventTile;
    }
    public Mass GetMassData(Vector3Int pos)
    {
        var mass = new Mass();
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);
        mass.isMovable = true;

        if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))
        {
            mass.isMovable = false;
        }
        else if(_tilemaps[BACKGROND_TILEMAP_NAME].GetTile(pos) == null)
        {
            mass.isMovable = false;
        }
        return mass;
    }
}
