using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopMenu : Menu
{
    [SerializeField] protected RPGSceneManager RPGSceneManager;
    [SerializeField] Text Description;

    public void Select()
    {
        var index = CurrentMenuObj.Index;
        if (index < 0 || ItemShop.Items.Count <= index) return;

        var item = ItemShop.Items[index];
        if (item == null) return;

        var messageWindow = RPGSceneManager.MessageWindow;
        var player = RPGSceneManager.Player;

        var yesNoMenu = messageWindow.YesNoMenu;
        if (yesNoMenu.DoOpen) return;

        yesNoMenu.YesAction = () =>
        {
            if (player.BattleParameter.IsLimitItemCount && !(item is Weapon))
            {
                messageWindow.Params = null;
                messageWindow.StartMessage(ItemShop.ItemCountOverMessage);
            }
            else if (player.BattleParameter.Money - item.Money >= 0)
            {
                player.BattleParameter.Money -= item.Money;
                if (item is Weapon)
                {
                    var weapon = item as Weapon;
                    switch (weapon.Kind)
                    {
                        case WeaponKind.Attack:
                            player.BattleParameter.AttackWeapon = weapon;
                            break;
                        case WeaponKind.Defense:
                            player.BattleParameter.DefenseWeapon = weapon;
                            break;
                    }
                }
                else
                {
                    player.BattleParameter.Items.Add(item);
                }

                messageWindow.Params = new string[] { player.BattleParameter.Money.ToString(), item.Money.ToString() };
                messageWindow.StartMessage(ItemShop.BuyMessage);
            }
            else
            {
                messageWindow.Params = new string[] { player.BattleParameter.Money.ToString() };
                messageWindow.StartMessage(ItemShop.NotEnoughMoneyMessage);
            }

            StartCoroutine(WaitInput());
        };

        yesNoMenu.NoAction = () => {
            EnableInput = true;
            messageWindow.Close();
        };

        EnableInput = false;
        messageWindow.Params = new string[] { item.Name, item.Money.ToString() };
        messageWindow.StartMessage(ItemShop.AskBuyMessage);
    }

    IEnumerator WaitInput()
    {
        //MessageWindowが閉じた時の操作がMenuコンポーネントの操作と被ってしまうので、回避策として用意したコルーチン
        EnableInput = false;
        var messageWindow = RPGSceneManager.MessageWindow;
        yield return new WaitWhile(() => messageWindow.gameObject.activeSelf);
        yield return null;
        EnableInput = true;
    }

    public ItemShopEvent ItemShop { get; private set; }
    public void Open(ItemShopEvent itemShop)
    {
        base.Open();
        ItemShop = itemShop;

        var menuItems = FirstMenuRoot.MenuItems;
        for (var i = 0; i < menuItems.Length; ++i)
        {
            var menuItem = menuItems[i];
            if (i < ItemShop.Items.Count)
            {
                menuItem.gameObject.SetActive(true);
                menuItem.Text = ItemShop.Items[i].Name;
            }
            else
            {
                menuItem.gameObject.SetActive(false);
            }
        }
    }

    protected override void ChangeMenuItem(MenuRoot menuRoot)
    {
        base.ChangeMenuItem(menuRoot);

        UpdateDescription();
    }

    void UpdateDescription()
    {
        var item = ItemShop.Items[CurrentMenuObj.Index];
        Description.text = item.Description;
    }
}