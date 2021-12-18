//LifeRecoveryEvent.cs LifeRecoveryEventクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/Life Recovery Event")]
public class LifeRecoveryEvent : MassEvent
{
    [TextArea(3, 15)] public string Message;
    [Min(0)] public int Money = 50;

    [TextArea(3, 15)] public string RecoveryMessage;
    [TextArea(3, 15)] public string NotEnoughMoneyMessage;
    [TextArea(3, 15)] public string NoMessage;


    public override void Exec(RPGSceneManager manager)
    {
        var messageWindow = manager.MessageWindow;
        var yesNoMenu = messageWindow.YesNoMenu;
        yesNoMenu.YesAction = () =>
        {
            var param = manager.Player.BattleParameter;
            if (param.Money - Money >= 0)
            {
                param.HP = param.MaxHP;
                param.Money -= Money;

                messageWindow.Params = new string[] { param.HP.ToString(), param.Money.ToString() };
                messageWindow.StartMessage(RecoveryMessage);
            }
            else
            {
                messageWindow.Params = new string[] { param.Money.ToString() };
                messageWindow.StartMessage(NotEnoughMoneyMessage);
            }
        };

        yesNoMenu.NoAction = () =>
        {
            messageWindow.Params = null;
            messageWindow.StartMessage(NoMessage);
        };

        messageWindow.Params = new string[] { Money.ToString() };
        manager.ShowMessageWindow(Message);
    }
}
