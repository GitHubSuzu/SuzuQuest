using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player;
    public Map ActiveMap;

    Coroutine _currentCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        _currentCoroutine = StartCoroutine(MovePlayer());
    }


    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (GetArrowInput(out var move))
            {
                var movedPos = Player.Pos + move; //現在地にmoveを加算
                var massData = ActiveMap.GetMassData(movedPos);
                Player.SetDir(move);////引数の値によって向きを変更
                if (massData.isMovable)//動けるのなら(isMovableがtrue)
                {
                    Player.Pos = movedPos;//プレイヤーの位置をmovedPosにする
                    yield return new WaitWhile(() => Player.IsMoving);//yield return new WaitWhile(条件) : 条件がfalseで再開

                    if (massData.massEvent != null)//nullじゃなかったら呼び出す
                    {
                        massData.massEvent.Exec(this);
                    }
                }
            }
            yield return null;
        }
    }

    //out を付けた引数で指定した変数はメソッド内で必ず結果が入る
    //boolで方向キーの入力があった場合は値を返す,それ以外はfalse
    bool GetArrowInput(out Vector3Int move)
    {
        var doMove = false;
        move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            move.x -= 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            move.x += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            move.y += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            move.y -= 1; doMove = true;
        }
        return doMove;
    }
}