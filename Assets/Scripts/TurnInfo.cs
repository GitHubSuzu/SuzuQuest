//TurnInfo.cs TurnInfoクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnInfo
{
    public string Message;
    public string[] Params;

    public UnityAction DoneCommand;
    public Animator[] Effects;
    public void ShowMessageWindow(MessageWindow messageWindow)
    {
        messageWindow.Params = Params;
        messageWindow.Effects = Effects;
        messageWindow.StartMessage(Message);
    }
}