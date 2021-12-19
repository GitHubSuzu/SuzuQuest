using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : Menu
{
    [SerializeField] protected RPGSceneManager RPGSceneManager;

    public void NewGame()
    {
        var savedData = Object.FindObjectOfType<SaveData>();
        savedData.ClearSave();
        RPGSceneManager.StartGame();

    }

    public void Continue()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        var savedData = Object.FindObjectOfType<SaveData>();
        if (!savedData.HaveSave)
        {
            RPGSceneManager.MessageWindow.StartMessage("セーブデータはありません...");
            yield return new WaitUntil(() => RPGSceneManager.MessageWindow.IsEndMessage);
            yield break;
        }

        RPGSceneManager.Player.IsActive = true;
        savedData.Load(RPGSceneManager);

        yield return new WaitWhile(() => savedData.NowLoading);

        if (savedData.IsSuccessLoad)
        {
            RPGSceneManager.StartGame();
        }
        else
        {
            RPGSceneManager.MessageWindow.StartMessage("セーブデータの読み込みに失敗しました...");
            yield return new WaitUntil(() => RPGSceneManager.MessageWindow.IsEndMessage);
        }

    }
}
