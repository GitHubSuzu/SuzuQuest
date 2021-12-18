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


    //次のものを追加
    public bool IsPauseScene
    {
        get
        {
            return !MessageWindow.IsEndMessage;
        }
    }
//次のものを修正
    IEnumerator MovePlayer()
    {
        while(true)
        {
            if (GetArrowInput(out var move))
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);
                Player.SetDir(move);
                if(massData.isMovable)
                {
                    Player.Pos = movedPos;
                    yield return new WaitWhile(() => Player.IsMoving);
 
                    if(massData.massEvent != null)
                    {
                        massData.massEvent.Exec(this);
                    }
                }
                else if(massData.character != null && massData.character.Event != null)
                {
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);
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
    
    //RPGSceneManager.cs 会話内容を表示するための処理を追加
    //次のものを追加（中身は後で記述）
    //RPGSceneManager.cs 会話内容を表示するための処理を追加
//次の部分を修正
    public MessageWindow MessageWindow;
    public void ShowMessageWindow(string message)
    {
        MessageWindow.StartMessage(message);
    }
}