//NPC.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : CharacterBase
{
    [SerializeField, Range(0.1f, 5f)] float WaitSecond = 1f;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(RandomMove());
    }

    public bool DoMovable = true;
    IEnumerator RandomMove()
    {
        var rnd = new System.Random();
        while (true)
        {
            yield return new WaitWhile(() => !DoMovable);
            
            yield return new WaitWhile(() => RPGSceneManager.IsPauseScene); //会話直後に移動させなくするためのもの
            
            var waitSecond = WaitSecond * (float)rnd.NextDouble();
            yield return new WaitForSeconds(waitSecond);
            
            yield return new WaitWhile(() => RPGSceneManager.IsPauseScene); //会話直後に移動させなくするためのもの

            var move = Vector3Int.zero;
            switch (rnd.Next() % 4)
            {
                case 0: move.x = -1; break;
                case 1: move.x = 1; break;
                case 2: move.y = -1; break;
                case 3: move.y = 1; break;
            }

            var movedPos = Pos + move;
            SetDir(move);
            if (RPGSceneManager.ActiveMap != null)
            {
                var massData = RPGSceneManager.ActiveMap.GetMassData(movedPos);
                if (massData.isMovable)
                {
                    Pos = movedPos;
                }
            }
            else
            {
                Pos += move;
            }
            yield return new WaitWhile(() => IsMoving);
        }
    }
}
