using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/Boss Event")]
public class BossEvent : MassEvent
{
    public EncounterEnemies EncounterEnemies;

    public override void Exec(RPGSceneManager manager)
    {
        var pos = manager.MassEventPos;
        var Boss = manager.ActiveMap.GetCharacter(pos) as Boss;
        if (Boss == null) return;

        manager.StartCoroutine(Battle(manager));
    }

    IEnumerator Battle(RPGSceneManager manager)
    {
        var pos = manager.MassEventPos;
        var Boss = manager.ActiveMap.GetCharacter(pos) as Boss;

        var battleWindow = manager.BattleWindow;
        battleWindow.SetUseEncounter(EncounterEnemies);
        battleWindow.Open();

        yield return new WaitWhile(() => battleWindow.DoOpen);

        if (manager.Player.BattleParameter.HP <= 0)
        {
            //Debug.Log("Fail Boss Battle...");
        }
        else
        {
            Boss.Kill();
            manager.GameClear();
        }
    }
}