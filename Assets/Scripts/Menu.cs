//Menu.cs Menuコンポーネントの本実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Menu : MonoBehaviour
{
    public MenuRoot FirstMenuRoot;

    public bool DoOpen { get => gameObject.activeSelf; }
    public virtual void Open()
    {
        gameObject.SetActive(true);
        StartCoroutine(UpdateWhenOpen());
    }

    public virtual void Close()
    {
        while(_menuRootStack.Count > 0)
        {
            _menuRootStack.Peek().IsActive = false;
            _menuRootStack.Pop();
        }
    }

    protected virtual void Awake()
    {
        gameObject.SetActive(false);
    }

    public MenuRoot CurrentMenuObj
    {
        get => _menuRootStack.Peek();
    }

    public MenuItem CurrentItem
    {
        get
        {
            var top = _menuRootStack.Peek();
            return top.CurrentItem;
        }
    }

    Stack<MenuRoot> _menuRootStack;
    public bool EnableInput { get; set; } = true;
    IEnumerator UpdateWhenOpen()
    {
        EnableInput = true;
        var menuRoots = GetComponentsInChildren<MenuRoot>();
        foreach(var root in menuRoots)
        {
            root.IsActive = false;
        }

        _menuRootStack = new Stack<MenuRoot>();
        _menuRootStack.Push(FirstMenuRoot);
        FirstMenuRoot.IsActive = true;

        yield return null;

        while (0 < _menuRootStack.Count)
        {
            var current = _menuRootStack.Peek();
            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                current.Index--;
                ChangeMenuItem(current);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                current.Index++;
                ChangeMenuItem(current);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                Decide(current);
            }
            else if(Input.GetKeyDown(KeyCode.Q))
            {
                Cancel(current);
            }
            yield return new WaitUntil(() => EnableInput);
        }
        gameObject.SetActive(false);
    }

    protected virtual void ChangeMenuItem(MenuRoot menuRoot)
    {}

    protected virtual void Decide(MenuRoot current)
    {
        var item = current.CurrentItem;
        switch (item.CurrentKind)
        {
            case MenuItem.Kind.NextMenu:
                if(item.MoveTargetObj.MenuItems.Length > 0)
                {
                    item.MoveTargetObj.IsActive = true;
                    _menuRootStack.Push(item.MoveTargetObj);
                    ChangeMenuItem(_menuRootStack.Peek());
                }
                break;
            case MenuItem.Kind.BackMenu:
                current.IsActive = false;
                _menuRootStack.Pop();
                if (_menuRootStack.Count > 0)
                {
                    ChangeMenuItem(_menuRootStack.Peek());
                }
                break;
            case MenuItem.Kind.Event:
                item.Callbacks.Invoke();
                break;
        }
    }

    protected virtual void Cancel(MenuRoot current)
    {
        current.IsActive = false;
        _menuRootStack.Pop();
        if (_menuRootStack.Count > 0)
        {
            ChangeMenuItem(_menuRootStack.Peek());
        }
    }
}