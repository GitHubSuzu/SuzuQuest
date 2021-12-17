using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    public Grid Grid { get => GetComponent<Grid>(); } //Girdを取得
    Dictionary<string, Tilemap> _tilemaps;            //タイルマップのDictionaryクラス

    readonly static string BACKGROND_TILEMAP_NAME = "BackGround";
    readonly static string NONE_OBJECTS_TILEMAP_NAME = "NoneObjects";
    readonly static string OBJECTS_TILEMAP_NAME = "Objects";
    readonly static string EVENT_BOX_TILEMAP_NAME = "EventBox";

    [SerializeField] List<MassEvent> _massEvents;
    public MassEvent FindMassEvent(TileBase tile)
    {
        return _massEvents.Find(_c => _c.Tile == tile);//設定されたListの中の設定されたTileアセットと引数で取得したtileが同じだったら返す
    }
    public bool FindMassEventPos(TileBase tile, out Vector3Int pos)
    {
        var eventLayer = _tilemaps[EVENT_BOX_TILEMAP_NAME];
        var renderer = eventLayer.GetComponent<Renderer>();
        var min = eventLayer.LocalToCell(renderer.bounds.min);
        var max = eventLayer.LocalToCell(renderer.bounds.max);
        pos = Vector3Int.zero;
        for (pos.y = min.y; pos.y < max.y; ++pos.y)
        {
            for (pos.x = min.x; pos.x < max.x; ++pos.x)
            {
                var t = eventLayer.GetTile(pos);
                if (t == tile) return true;
            }
        }
        return false;
    }

    private void Awake()
    {
        _tilemaps = new Dictionary<string, Tilemap>();//タイルマップのDictionaryクラス
        foreach (var tilemap in Grid.GetComponentsInChildren<Tilemap>())//Gridの子のTilemapコンポーネントを持つオブジェクトを格納
        {
            _tilemaps.Add(tilemap.name, tilemap);//Tilemapコンポーネントを持つオブジェクト名を格納
        }

        //EventBoxを非表示にする
        _tilemaps[EVENT_BOX_TILEMAP_NAME].gameObject.SetActive(false);
    }

    public Vector3 GetWorldPos(Vector3Int pos)
    {
        return Grid.CellToWorld(pos); //引数：Vector3 セル位置のワールド位置
    }

    public class Mass
    {
        public bool isMovable;
        public TileBase eventTile;
        public MassEvent massEvent;
    }
    public Mass GetMassData(Vector3Int pos)
    {
        var mass = new Mass();
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos);//タイルマップ上のタイルの位置
        mass.isMovable = true;//動ける
        
        if (mass.eventTile != null)
        {
            mass.massEvent = FindMassEvent(mass.eventTile);
        }
        else if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))
        {
            mass.isMovable = false;//動かない
        }
        else if (_tilemaps[BACKGROND_TILEMAP_NAME].GetTile(pos) == null)
        {
            mass.isMovable = false;//動けない
        }
        return mass;
    }
}