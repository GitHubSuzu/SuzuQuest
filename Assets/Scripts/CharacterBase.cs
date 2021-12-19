//CharacterBase.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    [Range(0, 2)] public float MoveSecond = 0.1f;
    [SerializeField] protected RPGSceneManager RPGSceneManager;

    Coroutine _moveCoroutine;
    [SerializeField] Vector3Int _pos;

    public MassEvent Event;

    Vector3Int InitialPos { get; set; }
    public bool IsActive { get => gameObject.activeSelf; set => gameObject.SetActive(value); }
    public string IdentityKey { get => $"{gameObject.name}_{GetType().Name}_{InitialPos}"; }

    public virtual Vector3Int Pos
    {
        get => _pos;
        set
        {
            if (_pos == value) return;

            if (RPGSceneManager.ActiveMap == null)
            {
                _pos = value;
            }
            else
            {
                if (_moveCoroutine != null)
                {
                    StopCoroutine(_moveCoroutine);
                    _moveCoroutine = null;
                }
                _moveCoroutine = StartCoroutine(MoveCoroutine(value));
            }
        }
    }
    public virtual void SetPosNoCoroutine(Vector3Int pos)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        _pos = pos;
        transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        MoveCamera();
    }
    public bool IsMoving { get => _moveCoroutine != null; }
    public bool DoMoveCamera = false;

    [SerializeField] Direction _currentDir = Direction.Down;
    public virtual Direction CurrentDir
    {
        get => _currentDir;
        set
        {
            if (_currentDir == value) return;
            _currentDir = value;
            SetDirAnimation(value);
        }
    }
    public virtual void SetDir(Vector3Int move)
    {
        if (Mathf.Abs(move.x) > Mathf.Abs(move.y))
        {
            CurrentDir = move.x > 0 ? Direction.Right : Direction.Left;
        }
        else
        {
            CurrentDir = move.y > 0 ? Direction.Up : Direction.Down;
        }
    }

    protected Animator Animator { get => GetComponent<Animator>(); }
    static readonly string TRIGGER_MoveDown = "MoveDownTrigger";
    static readonly string TRIGGER_MoveLeft = "MoveLeftTrigger";
    static readonly string TRIGGER_MoveRight = "MoveRightTrigger";
    static readonly string TRIGGER_MoveUp = "MoveUpTrigger";

    protected void SetDirAnimation(Direction dir)
    {
        if (Animator == null || Animator.runtimeAnimatorController == null) return;

        string triggerName = null;
        switch (dir)
        {
            case Direction.Up: triggerName = TRIGGER_MoveUp; break;
            case Direction.Down: triggerName = TRIGGER_MoveDown; break;
            case Direction.Left: triggerName = TRIGGER_MoveLeft; break;
            case Direction.Right: triggerName = TRIGGER_MoveRight; break;
            default: throw new System.NotImplementedException("");
        }
        Animator.SetTrigger(triggerName);
    }

    protected IEnumerator MoveCoroutine(Vector3Int pos)
    {
        _pos = pos;

        var startPos = transform.position;
        var goalPos = RPGSceneManager.ActiveMap.Grid.CellToWorld(pos);
        var t = 0f;
        while (t < MoveSecond)
        {
            yield return null;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, goalPos, t / MoveSecond);
            MoveCamera();
        }
        _moveCoroutine = null;
    }

    protected virtual void Awake()
    {
        if (RPGSceneManager == null) RPGSceneManager = Object.FindObjectOfType<RPGSceneManager>();

        InitialPos = Pos;
        SetDirAnimation(_currentDir);

        var joinedMap = GetJoinedMap();
        if (joinedMap != null)
        {
            joinedMap.AddCharacter(this);
        }
        else
        {
            RPGSceneManager.ActiveMap.AddCharacter(this);
        }
    }

    protected virtual void Start()
    {
        _moveCoroutine = StartCoroutine(MoveCoroutine(Pos));
    }

    protected void OnValidate()
    {
        if (RPGSceneManager != null && RPGSceneManager.ActiveMap != null)
        {
            transform.position = RPGSceneManager.ActiveMap.Grid.CellToWorld(Pos);
        }
        else if (transform.parent != null)
        {
            var map = transform.parent.GetComponent<Map>();
            if (map != null)
            {
                transform.position = map.Grid.CellToWorld(Pos);
            }
        }
    }

    public Map GetJoinedMap()
    {
        if (transform.parent != null)
        {
            return transform.parent.GetComponent<Map>();
        }
        else
        {
            return null;
        }
    }

    private Vector3 playerPivot = new Vector3(0.16f, 0.16f, 0.0f); //スプライトスライス時に中心からずれたピボット分の値
    private void MoveCamera()
    {
        if(DoMoveCamera == true) Camera.main.transform.position = transform.position + Vector3.forward * -10 + playerPivot;
    }

    [System.Serializable]
    public class InstantSaveData
    {
        public string id;
        public Vector3Int Pos;
        public Direction Direction;

        public InstantSaveData() { }
        public InstantSaveData(CharacterBase character)
        {
            id = character.IdentityKey;
            Pos = character.Pos;
            Direction = character.CurrentDir;
        }
    }

    public virtual InstantSaveData GetInstantSaveData()
    {
        return new InstantSaveData(this);
    }

    [System.Serializable]
    public class SaveData
    {
        public string id;

        public SaveData() { }
        public SaveData(CharacterBase character)
        {
            id = character.IdentityKey;
        }
    }

    public virtual SaveData GetSaveData()
    {
        return null;
    }

    public virtual void LoadInstantSaveData(string saveDataJson)
    {
        var saveData = JsonUtility.FromJson<InstantSaveData>(saveDataJson);
        SetPosNoCoroutine(saveData.Pos);
        CurrentDir = saveData.Direction;
    }

    public virtual void LoadSaveData(string saveDataJson)
    {

    }

}