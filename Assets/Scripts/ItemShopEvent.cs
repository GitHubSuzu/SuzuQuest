//ItemShopEvent.cs ItemShopEventクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MassEvent/ItemShopEvent")]
public class ItemShopEvent : MassEvent
{
    public List<Item> Items;

    [TextArea(3, 15)] public string Message;

    [TextArea(3, 15)] public string AskBuyMessage;
    [TextArea(3, 15)] public string BuyMessage;
    [TextArea(3, 15)] public string NotEnoughMoneyMessage;
    [TextArea(3, 15)] public string ItemCountOverMessage;
    [TextArea(3, 15)] public string CloseMessage;

    public override void Exec(RPGSceneManager manager)
    {
        var itemShop = manager.ItemShopMenu;
        itemShop.Open(this);
    }
}
