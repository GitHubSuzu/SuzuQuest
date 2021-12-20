// ItemList.cs ItemListの定義
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Item List")]
public class ItemList : ScriptableObject
{
    [SerializeField] List<Item> List;

    public int FindIndex(Item item)
    {
        return List.IndexOf(item);
    }

    public Item this[int index]
    {
        get => index != -1 ? List[index] : null;
    }
}