//EnemyStrongAI.cs EnemyStrongAIクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/AI/Strong AI")]
public class EnemyStrongAI : EnemyAI
{
    protected override void TurnAI(Enemy enemy, BattleWindow battleWindow, TurnInfo outTurnInfo)
    {
        if(Turn % 2 == 0)
        {
            outTurnInfo.Message = $"{enemy.Name}はなまけている...";
            outTurnInfo.DoneCommand = () => {};
        }
        else
        {
            outTurnInfo.Message = $"{enemy.Name}の攻撃!!";
            outTurnInfo.DoneCommand = () =>
            {
                enemy.Data.AttackTo(battleWindow.Player, out BattleParameterBase.AttackResult result);
                var messageWindow = battleWindow.GetRPGSceneManager.MessageWindow;
                var resultMsg = $"プレイヤーは{result.Damage}ダメージを受けた...";
                messageWindow.Params = null;
                messageWindow.StartMessage(resultMsg);
            };
        }
    }
}
