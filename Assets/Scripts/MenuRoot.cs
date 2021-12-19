using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuRoot : MonoBehaviour
{
    bool _isActive;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            UpdateMenuItemSelecting(!_isActive);
        }
    }

    int _index;
    public int Index
    {
        get
        {
            var items = MenuItems;
            _index = Mathf.Clamp(_index, 0, ActiveItemCount - 1);
            UpdateMenuItemSelecting(false);
            return _index;
        }
        set
        {
            _index = value;

            var items = MenuItems;
            _index = Mathf.Clamp(_index, 0, ActiveItemCount - 1);
            UpdateMenuItemSelecting(false);
            for (var i = 0; i < items.Length; ++i)
            {
                items[i].IsSelecting = i == _index;
            }
        }
    }

    public MenuItem CurrentItem
    {
        get
        {
            return MenuItems[Index];
        }
    }

    MenuItem[] _menuItems;
    public MenuItem[] MenuItems
    {
        get => _menuItems != null ? _menuItems : (_menuItems = GetComponentsInChildren<MenuItem>());
    }

    public void RefreshMenuItems(MenuItem[] menuItems)
    {
        _menuItems = menuItems;
    }

    public int ActiveItemCount
    {
        get => MenuItems.Count(_i => _i.gameObject.activeSelf);
    }

    void UpdateMenuItemSelecting(bool allDeactive)
    {
        var items = GetComponentsInChildren<MenuItem>();
        for (var i = 0; i < items.Length; ++i)
        {
            items[i].IsSelecting = allDeactive ? false : i == _index;
        }
    }

    private void Awake()
    {
        UpdateMenuItemSelecting(true);
    }
}
