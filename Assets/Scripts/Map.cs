using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using GlobalSaveData = SaveData;

public class Map : MonoBehaviour
{
    public Grid Grid { get => GetComponent<Grid>(); }
    Dictionary<string, Tilemap> _tilemaps;

    readonly static string BACKGROND_TILEMAP_NAME = "BackGround";
    readonly static string NONE_OBJECTS_TILEMAP_NAME = "NoneObjects";
    readonly static string OBJECTS_TILEMAP_NAME = "Objects";
    readonly static string EVENT_BOX_TILEMAP_NAME = "EventBox";

    [SerializeField] List<MassEvent> _massEvents;

    HashSet<CharacterBase> _characters = new HashSet<CharacterBase>();

    public RandomEncount RandomEncount;

    public void AddCharacter(CharacterBase character)
    {
        if (!_characters.Contains(character) && character != null)
        {
            _characters.Add(character);
        }
    }

    public CharacterBase GetCharacter(Vector3Int pos)
    {
        return _characters.Where(_c => _c.IsActive).FirstOrDefault(_c => _c.Pos == pos);
    }

    public MassEvent FindMassEvent(TileBase tile)
    {
        return _massEvents.Find(_c => _c.Tile == tile);
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
        _tilemaps = new Dictionary<string, Tilemap>();
        foreach (var tilemap in Grid.GetComponentsInChildren<Tilemap>())
        {
            _tilemaps.Add(tilemap.name, tilemap);
            var renderer = tilemap.GetComponent<Renderer>();
            var minCellPos = tilemap.LocalToCell(renderer.bounds.min);
            var maxCellPos = tilemap.LocalToCell(renderer.bounds.max);
        }

        //EventBoxを非表示にする
        _tilemaps[EVENT_BOX_TILEMAP_NAME].gameObject.SetActive(false);

        AddCharacter(Object.FindObjectOfType<Player>());
    }

    public Vector3 GetWorldPos(Vector3Int pos)
    {
        return Grid.CellToWorld(pos);
    }

    public class Mass
    {
        public bool isMovable;
        public TileBase eventTile;
        public MassEvent massEvent;
        public CharacterBase character;
    }
    public Mass GetMassData(Vector3Int pos)//引数の座標のマスの確認(タイル名,移動可能か)
    {
        var mass = new Mass();
        mass.eventTile = _tilemaps[EVENT_BOX_TILEMAP_NAME].GetTile(pos); //eventTile取得
        mass.isMovable = true; //プレイヤーが動けるか
        mass.character = GetCharacter(pos); //キャラクター取得

        if (mass.character != null)//characterがいるか判定
        {
            mass.isMovable = false;
        }
        else if (mass.eventTile != null)//eventTileか判定
        {
            mass.massEvent = FindMassEvent(mass.eventTile);
        }
        else if (_tilemaps[OBJECTS_TILEMAP_NAME].GetTile(pos))//Objectか判定
        {
            mass.isMovable = false;
        }
        else if (_tilemaps[BACKGROND_TILEMAP_NAME].GetTile(pos) == null)//BackGroundか判定
        {
            mass.isMovable = false;
        }
        return mass;
    }

    [System.Serializable]
    public class InstantSaveData
    {
        public List<string> characters = new List<string>();
    }

    public InstantSaveData GetInstantSaveData()
    {
        var saveData = new InstantSaveData();
        saveData.characters = _characters.Select(_c => _c.GetInstantSaveData()).Where(_s => _s != null).Select(_s => JsonUtility.ToJson(_s)).ToList();
        return saveData;
    }

    [System.Serializable]
    public class SaveData
    {
        public List<string> characters = new List<string>();
    }

    public SaveData GetSaveData()
    {
        var saveData = new SaveData();
        saveData.characters = _characters.Where(_c => !(_c is Player)).Select(_c => _c.GetSaveData()).Where(_s => _s != null).Select(_s => JsonUtility.ToJson(_s)).ToList();
        return saveData;
    }

    public CharacterBase GetCharacterId(string id)
    {
        return _characters.FirstOrDefault(_c => _c.IdentityKey == id);
    }

    public void Load(InstantSaveData saveData)
    {
        if (saveData.characters != null)
        {

            foreach (var json in saveData.characters)
            {
                var data = JsonUtility.FromJson<CharacterBase.SaveData>(json);
                if (data == null) continue;

                var ch = GetCharacterId(data.id);
                if (ch != null)
                {
                    ch.LoadInstantSaveData(json);
                }
                else
                {
                    Debug.LogError($"Don't found character={data.id}...");
                }
            }
        }
    }

    public void Load(SaveData saveData)
    {
        if (saveData.characters != null)
        {
            foreach (var json in saveData.characters)
            {
                var data = JsonUtility.FromJson<CharacterBase.SaveData>(json);
                if (data == null) continue;

                var ch = GetCharacterId(data.id);
                if (ch != null)
                {
                    ch.LoadSaveData(json);
                }
                else
                {
                    Debug.LogError($"Don't found character={data.id}...");
                }
            }
        }
    }

    private void Start()
    {
        var saveData = Object.FindObjectOfType<GlobalSaveData>();
        saveData.LoadSaveData(this);
    }

}