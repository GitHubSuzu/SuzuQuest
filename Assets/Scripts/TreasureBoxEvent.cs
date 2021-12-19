//TreasureBoxEvent.cs TreasureBoxEventクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/TreasureBox Event")]
public class TreasureBoxEvent : MassEvent
{
    public Item Item;
    [Min(1)] public int Count;

    [TextArea(3, 15)] public string OpenText = "宝箱を開けた";
    [TextArea(3, 15)] public string NotGetText = "アイテムがいっぱいだった...";
    [TextArea(3, 15)] public string GetText = "#0x#1を手に入れた!";
    [TextArea(3, 15)] public string GetWeaponText = "#0を手に入れた!";

    public override void Exec(RPGSceneManager manager)
    {
        var pos = manager.MassEventPos;
        var treasureBox = manager.ActiveMap.GetCharacter(pos) as TreasureBox;
        if (treasureBox == null) return;

        manager.StartCoroutine(OpenTreasure(manager));
    }

    IEnumerator OpenTreasure(RPGSceneManager manager)
    {
        var pos = manager.MassEventPos;
        var treasureBox = manager.ActiveMap.GetCharacter(pos) as TreasureBox;

        var messageWindow = manager.MessageWindow;
        messageWindow.Params = null;
        messageWindow.Effects = null;
        messageWindow.StartMessage(OpenText);

        yield return new WaitUntil(() => messageWindow.IsEndMessage);

        var player = manager.Player.BattleParameter;
        if (Item is Weapon)
        {
            messageWindow.Params = new string[] { Item.Name, Count.ToString() };
            messageWindow.StartMessage(GetWeaponText);
            yield return new WaitUntil(() => messageWindow.IsEndMessage);

            var weapon = Item as Weapon;
            switch (weapon.Kind)
            {
                case WeaponKind.Attack: player.AttackWeapon = weapon; break;
                case WeaponKind.Defense: player.DefenseWeapon = weapon; break;
            }
            treasureBox.Open();

        }
        else if (player.Items.Count + Count <= 4)
        {
            messageWindow.Params = new string[] { Item.Name, Count.ToString() };
            messageWindow.StartMessage(GetText);
            yield return new WaitUntil(() => messageWindow.IsEndMessage);
            for (var i = 0; i < Count; ++i)
            {
                player.Items.Add(Item);
            }
            treasureBox.Open();
        }
        else
        {
            messageWindow.StartMessage(NotGetText);
            yield return new WaitUntil(() => messageWindow.IsEndMessage);
        }
    }
}
