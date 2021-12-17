using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "MassEvent/Move Map")]
public class MoveMapEvent : MassEvent
{
    public Map MoveMapPrefab;        //移動先のMapPrefab
    public TileBase StartPosTile;    //移動先の開始位置
    public Direction StartDirection; //移動先の開始位置の向き

    public override void Exec(RPGSceneManager manager)
    {
        Destroy(manager.ActiveMap.gameObject);
        manager.ActiveMap = Instantiate(MoveMapPrefab);

        if (manager.ActiveMap.FindMassEventPos(StartPosTile, out var pos))
        {
            Debug.Log(pos);
            manager.Player.SetPosNoCoroutine(pos);
            manager.Player.CurrentDir = StartDirection;
        }
    }
}