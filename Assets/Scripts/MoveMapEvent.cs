using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MassEvent/Move Map")]
public class MoveMapEvent : MassEvent
{
    public Map MoveMapPrefab;
    public TileBase StartPosTile;
    public Direction StartDirection;

    public override void Exec(RPGSceneManager manager)
    {
        Object.Destroy(manager.ActiveMap.gameObject);
        manager.ActiveMap = Object.Instantiate(MoveMapPrefab);

        if (manager.ActiveMap.FindMassEventPos(StartPosTile, out var pos))
        {
            Debug.Log(pos);
            manager.Player.SetPosNoCoroutine(pos);
            manager.Player.CurrentDir = StartDirection;
        }
    }
}