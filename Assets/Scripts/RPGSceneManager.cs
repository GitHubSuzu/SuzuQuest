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

    public Menu Menu;
    public ItemShopMenu ItemShopMenu;
    public void OpenMenu()
    {
        Menu.Open();
    }
    
    [SerializeField] public BattleWindow BattleWindow;
    public Vector3Int MassEventPos { get; private set; }
    
    //次のものを追加
    public bool IsPauseScene
    {
        get
        {
            return !MessageWindow.IsEndMessage || Menu.DoOpen || ItemShopMenu.DoOpen || BattleWindow.DoOpen;
        }
    }
    //次のものを修正
    // RPGSceneManager.cs プレイヤーが移動したらランダムエンカウントするようにする
    IEnumerator MovePlayer()
    {
        var rnd = new System.Random();
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
                        MassEventPos = movedPos;
                        massData.massEvent.Exec(this);
                    }
                    else if(ActiveMap.RandomEncount != null)
                    {
                        rnd = new System.Random();
                        var encount = ActiveMap.RandomEncount.Encount(rnd);
                        if(encount != null)
                        {
                            BattleWindow.SetUseEncounter(encount);
                            BattleWindow.Open();
                        }
                    }
                }
                else if(massData.character != null && massData.character.Event != null)
                {
                    MassEventPos = movedPos;
                    massData.character.Event.Exec(this);
                }
            }
            yield return new WaitWhile(() => IsPauseScene);

            if(Input.GetKeyDown(KeyCode.Space))
            {
                OpenMenu();
            }
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