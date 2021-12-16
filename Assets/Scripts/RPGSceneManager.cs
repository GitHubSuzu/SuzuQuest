//RPGSceneManager.cs
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
        while(true)
        {
            if (GetArrowInput(out var move))
            {
                var movedPos = Player.Pos + move;
                var massData = ActiveMap.GetMassData(movedPos);
                if(massData.isMovable)
                {
                    Player.Pos = movedPos;
                    yield return new WaitWhile(() => Player.IsMoving);
                }
            }
            yield return null;
        }
    }

    bool GetArrowInput(out Vector3Int move)
    {
        var doMove = false;
        move = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log(1);
            move.x -= 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            move.x += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            move.y += 1; doMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            move.y -= 1; doMove = true;
        }
        return doMove;
    }
}