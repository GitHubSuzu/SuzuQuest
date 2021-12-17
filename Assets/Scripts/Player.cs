//Player.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction //動きの向き状態
{
    Up,
    Down,
    Left,
    Right,
}

public class Player : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f; //動く速さ
    [SerializeField] RPGSceneManager RPGSceneManager; //RPGSceneManager

    Coroutine _moveCoroutine;//StartCoroutine(MoveCoroutine(Pos));
    [SerializeField] Vector3Int _pos; //プレイヤーの位置

    private Vector3 playerPivot = new Vector3(0.16f, 0.16f, 0.0f); //スプライトスライス時に中心からずれたピボット分の値

    [SerializeField] Direction _currentDir = Direction.Down; //現在の向き(デフォルト下)
    public Direction CurrentDir //現在の状態確認する関数
    {
        get => _currentDir;
        set
        {
            //現在の向きを確認（同じの場合はreturnで返す、違う場合はvalueに格納しSetDirAnimation(value)を呼ぶ）
            if (_currentDir == value) return; 
            _currentDir = value;
            SetDirAnimation(value);
        }
    }
    public void SetDir(Vector3Int move)//引数の値によって向きを変更
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))//絶対値による比較
        {
            //?: 演算子 ： trueの場合は左、falseの場合は右
            CurrentDir = move.x > 0 ? Direction.Right : Direction.Left;//値によって向きを変更
        }
        else
        {
            CurrentDir = move.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    Animator Animator { get => GetComponent<Animator>(); }//Animatorコンポーネント取得
    //readonly(実行時定数)
    //SetTriggerのキーを事前にHash値としてキャッシュする
    static readonly string TRIGGER_MoveDown = "MoveDownTrigger";
    static readonly string TRIGGER_MoveLeft = "MoveLeftTrigger";
    static readonly string TRIGGER_MoveRight = "MoveRightTrigger";
    static readonly string TRIGGER_MoveUp = "MoveUpTrigger";

    void SetDirAnimation(Direction dir) //引数に格納されている向きによってアニメーションをする
    {
        if (Animator == null || Animator.runtimeAnimatorController == null) return; //nullだったらreturnで返す

        string triggerName = null; //トリガー名を格納
        switch (dir)
        {
            case Direction.Up: triggerName = TRIGGER_MoveUp; break;
            case Direction.Down: triggerName = TRIGGER_MoveDown; break;
            case Direction.Left: triggerName = TRIGGER_MoveLeft; break;
            case Direction.Right: triggerName = TRIGGER_MoveRight; break;
            default: throw new System.NotImplementedException(""); //nullだったらエラーを返す
        }
        Animator.SetTrigger(triggerName);//指定された向きのアニメーション起動
    }

    private void Awake()
    {
        SetDirAnimation(_currentDir);//実行時からアニメーションしておくため呼び出す
    }

    public Vector3Int Pos
    {
        get => _pos; //プレイヤー位置を取得
        set
        {
            if (_pos == value) return; //同じ場合はreturnで返す

            if (RPGSceneManager.ActiveMap == null)
            {
                _pos = value;//位置を格納
            }
            else
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);//コルーチンを止めてnullにする
                    _moveCoroutine = null;
                    Debug.Log(1);
                }
                _moveCoroutine = StartCoroutine(MoveCoroutine(value));//nullだったら現在を引数にして呼び出す
            }

        }
    }
    public void SetPosNoCoroutine(Vector3Int pos)
    {
        _pos = pos;
        transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);//引数：Vector3 セル位置のワールド位置
        Camera.main.transform.position = transform.position + Vector3.forward * -10 + playerPivot;
    }
    public bool IsMoving { get => _moveCoroutine != null; }

    IEnumerator MoveCoroutine(Vector3Int pos)
    {
        var startPos = transform.position;
        var goalPos = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos); //引数：Vector3 セル位置のワールド位置
        var t = 0f;
        while (t < MoveSecond)
        {
            yield return null;
            t += Time.deltaTime;//Time.deltaTime：直前のフレームと今のフレーム間で経過した時間
            transform.position = Vector3.Lerp(startPos, goalPos, t / MoveSecond);//Vector3.Lerp(始まりの位置, 終わりの位置, 現在の位置)
            Camera.main.transform.position = transform.position + Vector3.forward * -10 + playerPivot;
        }
        _pos = pos;
        _moveCoroutine = null;

    }

    private void Start()
    {
        if (RPGSceneManager == null) RPGSceneManager = FindObjectOfType<RPGSceneManager>();

        _moveCoroutine = StartCoroutine(MoveCoroutine(Pos));
    }

    private void OnValidate()
    {
        if (RPGSceneManager != null && RPGSceneManager.ActiveMap != null)
        {
            transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(Pos); //引数：Vector3 セル位置のワールド位置
        }
    }
}