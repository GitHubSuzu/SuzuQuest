//EnemyBasicAI.cs EnemyBasicAIクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Enemy/AI/Basic AI")]
public class EnemyBasicAI : EnemyAI
{
    protected override void TurnAI(Enemy enemy, BattleWindow battleWindow, TurnInfo outTurnInfo)
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
