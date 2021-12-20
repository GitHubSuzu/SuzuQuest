using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BattleWindow : Menu
{
    [SerializeField] RPGSceneManager RPGSceneManager;
    [SerializeField] MenuRoot MainCommands;
    [SerializeField] MenuRoot Items;
    [SerializeField] MenuRoot Enemies;
    [SerializeField] MenuItem EnemyPrefab;
    [SerializeField] Text Description;
    [SerializeField] GameObject ParameterRoot;

    [SerializeField] EncounterEnemies UseEncounter;
    public EncounterEnemies Encounter { get; private set; }

    [SerializeField] Animator AttackEffectPrefab;
    Animator AttackEffect;

    public BattleParameterBase Player { get => RPGSceneManager.Player.BattleParameter; }
    public RPGSceneManager GetRPGSceneManager { get => RPGSceneManager; }

    public void SetUseEncounter(EncounterEnemies encounter) { UseEncounter = encounter; }

    void SetupEnemies()
    {
        foreach (var e in Enemies.MenuItems)
        {
            Object.Destroy(e.gameObject);
        }

        var newEnemies = new List<MenuItem>();
        foreach (var e in Encounter.Enemies)
        {
            var enemy = Object.Instantiate(EnemyPrefab, Enemies.transform);
            Debug.Log(e.Name);
            enemy.Text = e.Name;
            var image = enemy.transform.Find("Image").GetComponent<Image>();
            image.sprite = e.Sprite;

            enemy.CurrentKind = MenuItem.Kind.Event;
            enemy.Callbacks.AddListener(this.Attack);
            newEnemies.Add(enemy);
        }
        Enemies.RefreshMenuItems(newEnemies.ToArray());
    }
    public override void Open()
    {
        var saveData = Object.FindObjectOfType<SaveData>();
        saveData.SaveTemporary(RPGSceneManager.ActiveMap);

        base.Open();
        MainCommands.Index = 0;
        DoEscape = false;
        Items.gameObject.SetActive(false);
        Description.transform.parent.gameObject.SetActive(false);
        UpdateUI();

        Encounter = UseEncounter.Clone();
        SetupEnemies();
    }

    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        base.ChangeMenuItem(menuRoot);

        Enemies.gameObject.SetActive(true);
        Items.gameObject.SetActive(CurrentMenuObj == Items);

        var player = RPGSceneManager.Player;
        if (CurrentMenuObj == Items && 0 <= CurrentMenuObj.Index && CurrentMenuObj.Index < player.BattleParameter.Items.Count)
        {

            Description.transform.parent.gameObject.SetActive(true);
            var item = player.BattleParameter.Items[CurrentMenuObj.Index];
            Description.text = item.Description;
        }
        else
        {
            Description.transform.parent.gameObject.SetActive(false);
        }
    }

    void UpdateUI()
    {
        UpdateParameters();
        UpdateItem(RPGSceneManager.Player.BattleParameter);
    }

    void UpdateParameters()
    {
        var player = RPGSceneManager.Player;
        var param = player.BattleParameter;
        SetParameterText("HP", $"{param.HP}/{param.MaxHP}");
        SetParameterText("ATK", param.AttackPower.ToString());
        SetParameterText("DEF", param.DefensePower.ToString());
    }

    void SetParameterText(string name, string text)
    {
        var root = ParameterRoot.transform.Find(name);
        var textObj = root.Find("Text").GetComponent<Text>();
        textObj.text = text;
    }

    void UpdateItem(BattleParameterBase param)
    {
        var items = param.Items;
        var useItems = new List<MenuItem>();
        var menuItems = Items.GetComponentsInChildren<MenuItem>(true);
        for (var i = 0; i < menuItems.Length; ++i)
        {
            var menuItem = menuItems[i];
            if (i < items.Count)
            {
                menuItem.gameObject.SetActive(true);
                menuItem.Text = items[i].Name;
                useItems.Add(menuItem);
            }
            else
            {
                menuItem.gameObject.SetActive(false);
            }
        }
        Items.RefreshMenuItems(useItems.ToArray());
    }

    protected override void Cancel(MenuRoot current)
    {
        if (CurrentMenuObj != MainCommands)
        {
            base.Cancel(current);
        }
    }

    public void Attack()
    {
        var enemyIndex = CurrentMenuObj.Index;
        var enemy = Encounter.Enemies[enemyIndex];

        var turnInfo = new TurnInfo();
        turnInfo.Message = $"{enemy.Name}にこうげき！\n"
            + $"<ANIMATION> 0 Attack"
            ;
        AttackEffect = Object.Instantiate(AttackEffectPrefab, CurrentItem.transform);
        turnInfo.Effects = new Animator[] { AttackEffect };

        turnInfo.DoneCommand = () => {
            var player = RPGSceneManager.Player.BattleParameter;
            BattleParameterBase.AttackResult result;
            var doKill = player.AttackTo(enemy.Data, out result);

            var messageWindow = RPGSceneManager.MessageWindow;
            var resultMsg = $"{enemy.Name}に{result.Damage}を与えた！";
            if (doKill)
            {
                resultMsg += $"\n{enemy.Name}を倒した！!";
                Encounter.Enemies.RemoveAt(enemyIndex);
                SetupEnemies();
            }
            messageWindow.Params = null;
            messageWindow.StartMessage(resultMsg);
        };
        StartTurn(turnInfo);
    }

    public void Defense()
    {
        var turnInfo = new TurnInfo();
        turnInfo.Message = "ぼうぎょした！";
        turnInfo.DoneCommand = () => {
            RPGSceneManager.Player.BattleParameter.IsNowDefense = true;
        };
        StartTurn(turnInfo);
    }

    public void UseItem()
    {
        Items.gameObject.SetActive(false);

        var player = RPGSceneManager.Player.BattleParameter;
        var itemIndex = CurrentMenuObj.Index;
        var useItem = player.Items[itemIndex];

        var turnInfo = new TurnInfo();
        turnInfo.Message = $"{useItem.Name}を使った!";
        turnInfo.DoneCommand = () =>
        {
            var messageWindow = RPGSceneManager.MessageWindow;
            if (useItem is Weapon)
            {
                messageWindow.StartMessage($"{useItem.Name}は使えないよ...");
            }
            else
            {
                useItem.Use(player);
                messageWindow.StartMessage($"{useItem.Name}はなくなった...");
                player.Items.RemoveAt(itemIndex);
            }
        };
        StartTurn(turnInfo);
    }

    bool DoEscape { get; set; }
    [Min(0)] public float EscapeWaitSecond = 1f;

    public void Escape()
    {
        var turnInfo = new TurnInfo();
        turnInfo.Message = "にげようとした！";
        turnInfo.DoneCommand = () => {
            var messageWindow = RPGSceneManager.MessageWindow;
            var rnd = new System.Random();
            DoEscape = (float)rnd.NextDouble() < Encounter.EscapeSuccessRate;
            if (DoEscape)
            {
                messageWindow.StartMessage("うまくにげれた!");
            }
            else
            {
                messageWindow.StartMessage("つかまってしまった...");
            }
        };
        StartTurn(turnInfo);
    }

    void StartTurn(TurnInfo turnInfo)
    {
        if (_turnCoroutine != null) return;
        while (CurrentMenuObj != MainCommands)
        {
            Cancel(CurrentMenuObj);
        }
        EnableInput = false;

        _turnCoroutine = StartCoroutine(Turn(turnInfo));
    }

    Coroutine _turnCoroutine;
    IEnumerator Turn(TurnInfo turnInfo)
    {
        var messageWindow = RPGSceneManager.MessageWindow;
        turnInfo.ShowMessageWindow(messageWindow);
        yield return new WaitWhile(() => !messageWindow.IsEndMessage);
        if (AttackEffect != null) { Object.Destroy(AttackEffect.gameObject); }
        if (turnInfo.DoneCommand != null) turnInfo.DoneCommand();
        yield return new WaitWhile(() => !messageWindow.IsEndMessage);
        UpdateUI();

        if (DoEscape)
        {
            yield return new WaitForSeconds(EscapeWaitSecond);
            Close();
        }
        else
        {
            foreach (var enemy in Encounter.Enemies)
            {
                var info = enemy.BattleAction(this);
                info.ShowMessageWindow(messageWindow);
                yield return new WaitWhile(() => !messageWindow.IsEndMessage);
                if (info.DoneCommand != null) info.DoneCommand();
                yield return new WaitWhile(() => !messageWindow.IsEndMessage);
                UpdateUI();
            }
        }

        var player = RPGSceneManager.Player.BattleParameter;
        if (player.HP <= 0)
        {
            messageWindow.StartMessage($"負けてしまった...");
            yield return new WaitWhile(() => !messageWindow.IsEndMessage);
            Close();
            
        }
        else if (Encounter.Enemies.Count <= 0)
        {
            var exp = UseEncounter.Enemies.Sum(_e => _e.Data.Exp);
            var money = UseEncounter.Enemies.Sum(_e => _e.Data.Money);
            var msg = $"戦闘に勝った！\n"
                + $"Exp+{exp}かくとく!\n"
                + $"お金+${money}かくとく!";
            messageWindow.StartMessage(msg);
            yield return new WaitWhile(() => !messageWindow.IsEndMessage);

            player.Money += money;
            var prevLevel = player.Level;
            if (player.GetExp(exp))
            {
                msg = $"レベルが上がった！ {prevLevel} -> {player.Level}\n"
                    + $"  MaxHP -> {player.MaxHP}\n"
                    + $"  ATK -> {player.AttackPower}\n"
                    + $"  DEF -> {player.DefensePower}"
                    ;
                messageWindow.StartMessage(msg);
                yield return new WaitWhile(() => !messageWindow.IsEndMessage);
            }
            Close();
        }

        TurnEndProcess();
        _turnCoroutine = null;
    }

    void TurnEndProcess()
    {
        RPGSceneManager.Player.BattleParameter.IsNowDefense = false;
        EnableInput = true;
    }
}