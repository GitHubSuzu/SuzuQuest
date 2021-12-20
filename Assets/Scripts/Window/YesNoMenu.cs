//YesNoMenu.cs YesNoMenuコンポーネントの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class YesNoMenu : Menu
{
    UnityAction _yesAction;
    UnityAction _noAction;
    public UnityAction YesAction { private get => _yesAction; set => _yesAction = value; }
    public UnityAction NoAction { private get => _noAction; set => _noAction = value; }

    public void Yes()
    {
        YesAction?.Invoke();
        YesAction = null;
        Close();
    }

    public void No()
    {
        NoAction?.Invoke();
        NoAction = null;
        Close();
    }

    protected override void Cancel(MenuRoot current) {}
}