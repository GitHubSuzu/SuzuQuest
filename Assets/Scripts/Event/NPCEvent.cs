//NPCEvent.cs 会話用のイベントクラスの定義
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/NPC Event")]
    public class NPCEvent : MassEvent
{
    [TextArea(3, 15)]public string Message;
    public override void Exec(RPGSceneManager manager)
    {
        manager.ShowMessageWindow(Message);
    }
}