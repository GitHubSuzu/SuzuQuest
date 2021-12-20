using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGSceneManager : MonoBehaviour
{
    public Player Player; //PlayerScript
    public Map ActiveMap; //現在のMapScript
    public MessageWindow MessageWindow; //会話WindowScript
    public Menu Menu; //メニュー(アイテム,セーブロード)WindowScript
    public ItemShopMenu ItemShopMenu; //アイテムショップwindowScript

    Coroutine _currentCoroutine;

    [SerializeField] public BattleWindow BattleWindow; //戦闘Script

    public Vector3Int MassEventPos { get; private set; }//Event用座標

    public TitleMenu TitleMenu; //タイトルWindowScript

    public ItemList ItemList; //ゲーム内のアイテム管理リストScript
    public void StartTitle()
    {
        StopCurrentCoroutine();
        Player.gameObject.SetActive(false);
        if (ActiveMap != null) ActiveMap.gameObject.SetActive(false);
        TitleMenu.Open();
    }

    public void StartGame()
    {
        StopCurrentCoroutine();
        TitleMenu.Close();
        Player.gameObject.SetActive(true);
        if (ActiveMap != null) ActiveMap.gameObject.SetActive(true);
        _currentCoroutine = StartCoroutine(MovePlayer());
    }

    void StopCurrentCoroutine()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
            _currentCoroutine = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartTitle();
    }


    IEnumerator MovePlayer()
    {
        while (true)
        {
            if (GetArrowInput(out var move))//引数：動く座標(向き) 例：Vector3.x = -1
            {
                var movedPos = Player.Pos + move; //プレイヤーの座標にmoveを加算
                var massData = ActiveMap.GetMassData(movedPos);//移動先のMassを確認
                Player.SetDir(move);//向きの状態
                if (massData.isMovable)//動けるか判定
                {
                    Player.Pos = movedPos;//プレイヤーの座標をmovedposの座標にする
                    yield return new WaitWhile(() => Player.IsMoving);

                    if (massData.massEvent != null)//massEventか判定
                    {
                        MassEventPos = movedPos;//MassEventPosに座標を格納
                        massData.massEvent.Exec(this);//MassEventのExceを呼び出す
                    }

                    else if (ActiveMap.RandomEncount != null)//RandomEncountがあるか判定
                    {
                        var rnd = new System.Random();
                        var encount = ActiveMap.RandomEncount.Encount(rnd);//出現するEnemyの種類が返ってくる
                        if (encount != null)//Enemyに遭遇するか判定
                        {
                            BattleWindow.SetUseEncounter(encount);//encountに格納されている出現するEnemyの種類を格納
                            BattleWindow.Open();//BattleWindow.Openメソッドを呼び出す
                        }
                    }

                }
                else if (massData.character != null && massData.character.Event != null)//キャラクター且つイベントだったら
                {
                    MassEventPos = movedPos; //MassEventPosに座標を格納
                    massData.character.Event.Exec(this); //Character.EventのExceを呼び出す
                }
            }
            yield return new WaitWhile(() => IsPauseScene);

            if (Player.BattleParameter.HP <= 0)//HPが0以下か判定
            {
                StartCoroutine(GameOver());//GameOverメソッドを呼び出す
                yield break;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                OpenMenu();//OpenMenuメソッドを呼び出す
            }
        }
    }

    bool GetArrowInput(out Vector3Int move)//方向キー入力があった場合tureを返して、動く座標(向き)を返す
    {
        var doMove = false;
        move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            move.x -= 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            move.x += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            move.y += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            move.y -= 1; doMove = true;
        }
        return doMove;
    }

    public void ShowMessageWindow(string message)
    {
        MessageWindow.StartMessage(message);
    }

    public bool IsPauseScene
    {
        get
        {
            return !MessageWindow.IsEndMessage || Menu.DoOpen || ItemShopMenu.DoOpen || BattleWindow.DoOpen;
        }
    }

    public void OpenMenu()
    {
        Menu.Open();//Menu.Openメソッドを呼び出す
    }

    [SerializeField, TextArea(3, 15)] string GameOverMessage = "体力が無くなった...";
    [SerializeField] Map RespawnMapPrefab;  //リスポーンマップScript
    [SerializeField] Vector3Int RespawnPos; //リスポーン地点の座標
    IEnumerator GameOver()
    {
        MessageWindow.StartMessage(GameOverMessage);//MessageWindow.StartMessageメソッドを呼び出す
        yield return new WaitUntil(() => MessageWindow.IsEndMessage);

        RespawnMap(true);//RespawnMapメソッドを呼び出す
    }

    void RespawnMap(bool isGameOver)
    {
        Destroy(ActiveMap.gameObject);//現在のマップを削除
        ActiveMap = Instantiate(RespawnMapPrefab);//リスポーンマップを生成

        Player.SetPosNoCoroutine(RespawnPos);//Player.SetPosNoCoroutineメソッドを呼び出す
        Player.CurrentDir = Direction.Down;//プレイヤーの向きを指定
        if (isGameOver)
        {
            Player.BattleParameter.HP = 1;
            Player.BattleParameter.Money = 100;
        }

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
        _currentCoroutine = StartCoroutine(MovePlayer());
    }

    public void GameClear()
    {
        StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(GameClearCoroutine());//GameClearCoroutineメソッドを呼び出す
    }

    [SerializeField, TextArea(3, 15)] string GameClearMessage = "ゲームクリアー";
    [SerializeField] GameClear gameClearObj;
    IEnumerator GameClearCoroutine()
    {
        MessageWindow.StartMessage(GameClearMessage);//MessageWindow.StartMessageメソッドを呼び出す
        yield return new WaitUntil(() => MessageWindow.IsEndMessage);

        gameClearObj.StartMessage(gameClearObj.Message);//gameClearObj.StartMessageメソッドを呼び出す
        yield return new WaitWhile(() => gameClearObj.DoOpen);

        _currentCoroutine = null;
        RespawnMap(false);//RespawnMapメソッドを呼び出す
    }
}