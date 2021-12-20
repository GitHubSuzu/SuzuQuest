using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MainMenu : Menu
{
    [SerializeField] protected RPGSceneManager RPGSceneManager;

    public GameObject ParameterRoot;
    public MenuRoot ItemInventory;

    Item GetItem(BattleParameterBase param, int index)
    {
        int i = 0;
        if (param.AttackWeapon != null)
        {
            if (index == i) return param.AttackWeapon;
            i++;
        }
        if (param.DefenseWeapon != null)
        {
            if (index == i) return param.DefenseWeapon;
            i++;
        }

        index -= i;
        for (var itemIndex = 0; itemIndex < param.Items.Count; ++itemIndex)
        {
            if (index == itemIndex) return param.Items[itemIndex];
        }
        return null;
    }

    public void UseItem()
    {
        var index = CurrentMenuObj.Index;
        var player = RPGSceneManager.Player;
        var item = GetItem(player.BattleParameter, index);
        if (item == null || item is Weapon) return;
        item.Use(player.BattleParameter);
        int offset = 0;
        if (player.BattleParameter.AttackWeapon != null) offset++;
        if (player.BattleParameter.DefenseWeapon != null) offset++;
        player.BattleParameter.Items.RemoveAt(index - offset);
        UpdateUI();
    }

    public override void Open()
    {
        base.Open();
        UpdateUI();
    }

    void UpdateUI()
    {
        UpdateItems();
        UpdateParameters();
        UpdateDescription();
    }

    void UpdateItems()
    {
        //アイテムは6個までを上限と想定して作成しています。(武器+防具+アイテム4個)
        var player = RPGSceneManager.Player;
        var items = player.BattleParameter.Items;
        var menuItems = ItemInventory.MenuItems;
        foreach (var menuItem in menuItems)
        {
            menuItem.gameObject.SetActive(false);
        }

        int i = 0;
        if (player.BattleParameter.AttackWeapon != null)
        {
            menuItems[i].gameObject.SetActive(true);
            menuItems[i].Text = player.BattleParameter.AttackWeapon.Name;
            i++;
        }
        if (player.BattleParameter.DefenseWeapon != null)
        {
            menuItems[i].gameObject.SetActive(true);
            menuItems[i].Text = player.BattleParameter.DefenseWeapon.Name;
            i++;
        }

        for (var itemIndex = 0; i < menuItems.Length && itemIndex < items.Count; ++i, ++itemIndex)
        {
            var menuItem = menuItems[i];
            menuItem.gameObject.SetActive(true);
            menuItem.Text = items[itemIndex].Name;
        }
    }

    void UpdateParameters()
    {
        var player = RPGSceneManager.Player;
        var param = player.BattleParameter;
        SetParameterText("LEVEL", param.Level.ToString());
        SetParameterText("EXP", param.Exp.ToString());
        SetParameterText("HP", $"{param.HP}/{param.MaxHP}");
        SetParameterText("ATK", param.Attack.ToString());
        SetParameterText("DEF", param.Defense.ToString());
        SetParameterText("MONEY", param.Money.ToString());
    }

    void SetParameterText(string name, string text)
    {
        var root = ParameterRoot.transform.Find(name);
        var textObj = root.Find("Text").GetComponent<Text>();
        textObj.text = text;
    }

    public Text Description;

    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        UpdateDescription();
    }

    void UpdateDescription()
    {
        if (CurrentMenuObj == ItemInventory)
        {
            Description.transform.parent.gameObject.SetActive(true);
            Description.text = GetItem(RPGSceneManager.Player.BattleParameter, CurrentMenuObj.Index).Description;
        }
        else
        {
            Description.transform.parent.gameObject.SetActive(false);
        }
    }

    public void Save()
    {
        StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        var saveData = Object.FindObjectOfType<SaveData>();
        saveData.Save(RPGSceneManager);

        EnableInput = false;
        RPGSceneManager.MessageWindow.StartMessage("セーブしました。");

        yield return new WaitUntil(() => RPGSceneManager.MessageWindow.IsEndMessage);
        EnableInput = true;
    }

    public void Load()
    {
        StartCoroutine(LoadCoroutine());
    }

    IEnumerator LoadCoroutine()
    {
        var saveData = Object.FindObjectOfType<SaveData>();
        saveData.Load(RPGSceneManager);

        yield return new WaitWhile(() => saveData.NowLoading);

        EnableInput = false;
        RPGSceneManager.MessageWindow.StartMessage(saveData.IsSuccessLoad ? "ロードしました。" : "ロードに失敗しました...");

        yield return new WaitUntil(() => RPGSceneManager.MessageWindow.IsEndMessage);
        EnableInput = true;
        Close();
    }

}