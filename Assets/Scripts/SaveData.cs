using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    [SerializeField] List<Map> _maps;
    public bool NowLoading { get; private set; } = false;
    public bool IsSuccessLoad { get; private set; } = false;

    public void Load(RPGSceneManager manager)
    {
        StopAllCoroutines();
        StartCoroutine(LoadCoroutine(manager));
    }

    IEnumerator LoadCoroutine(RPGSceneManager manager)
    {
        IsSuccessLoad = false;
        NowLoading = false;
        if (!PlayerPrefs.HasKey("player"))
        {
            Debug.LogError("Fail to Load saveData");
            yield break;
        }

        NowLoading = true;
        var mapName = PlayerPrefs.GetString("activeMap");
        var loadMap = _maps.Find(_map => _map.name.Replace("(Clone)", "") == mapName);
        if (loadMap == null)
        {
            Debug.LogError($"Fail to find map({mapName})... ");
            yield break;
        }

        DeleteTemporarySaveDatas();

        Object.Destroy(manager.ActiveMap.gameObject);
        manager.ActiveMap = Object.Instantiate(loadMap);

        yield return null;

        var player = manager.Player;
        player.LoadSaveData(PlayerPrefs.GetString("player"));

        var instantMapData = JsonUtility.FromJson<Map.InstantSaveData>(PlayerPrefs.GetString("instantMapData"));
        manager.ActiveMap.Load(instantMapData);

        var mapData = JsonUtility.FromJson<Map.SaveData>(PlayerPrefs.GetString($"map_{mapName}"));
        manager.ActiveMap.Load(mapData);
        NowLoading = false;
        IsSuccessLoad = true;
    }

    public void LoadSaveData(Map map)
    {
        var key = $"map_{map.name.Replace("(Clone)", "")}";
        var tempKey = "temp_" + key;
        if (PlayerPrefs.HasKey(tempKey))
        {
            Debug.Log($"{map.name}: Load Temp SaveData\n{PlayerPrefs.GetString(tempKey)}");
            var mapData = JsonUtility.FromJson<Map.SaveData>(PlayerPrefs.GetString(tempKey));
            map.Load(mapData);
        }
        else if (PlayerPrefs.HasKey(key))
        {
            Debug.Log($"{map.name}: Load SaveData \n{PlayerPrefs.GetString(key)}");
            var mapData = JsonUtility.FromJson<Map.SaveData>(PlayerPrefs.GetString(key));
            map.Load(mapData);
        }
    }

    private void Awake()
    {
        DeleteTemporarySaveDatas();
    }

    public void DeleteTemporarySaveDatas()
    {
        foreach (var map in _maps)
        {
            var key = $"temp_map_{map.name.Replace("(Clone)", "")}";
            PlayerPrefs.DeleteKey(key);
        }
    }

    public void Save(RPGSceneManager manager)
    {
        PlayerPrefs.SetString("player", JsonUtility.ToJson(manager.Player.GetSaveData()));
        var mapName = manager.ActiveMap.name;
        mapName = mapName.Replace("(Clone)", "");
        PlayerPrefs.SetString("activeMap", mapName);
        var instantMapData = manager.ActiveMap.GetInstantSaveData();
        PlayerPrefs.SetString("instantMapData", JsonUtility.ToJson(instantMapData));

        var activeMapKey = $"map_{manager.ActiveMap.name.Replace("(Clone)", "")}";
        foreach (var map in _maps)
        {
            var key = $"map_{map.name.Replace("(Clone)", "")}";
            if (key == activeMapKey)
            {
                Save(manager.ActiveMap);
            }
            else
            {
                SaveWithTemporary(map);
            }
        }
    }

    void Save(Map map)
    {
        var key = $"map_{map.name.Replace("(Clone)", "")}";
        PlayerPrefs.SetString(key, JsonUtility.ToJson(map.GetSaveData()));
    }

    void SaveWithTemporary(Map map)
    {
        var key = $"map_{map.name.Replace("(Clone)", "")}";
        var tempKey = "temp_" + key;
        if (PlayerPrefs.HasKey(tempKey))
        {
            PlayerPrefs.SetString(key, PlayerPrefs.GetString(tempKey));
            PlayerPrefs.DeleteKey(tempKey);
        }
    }

    public void SaveTemporary(Map map)
    {
        var saveData = map.GetSaveData();
        PlayerPrefs.SetString($"temp_map_{map.name.Replace("(Clone)", "")}", JsonUtility.ToJson(saveData));
    }

    public bool HaveSave
    {
        get => PlayerPrefs.HasKey("player");
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteAll();
    }

}