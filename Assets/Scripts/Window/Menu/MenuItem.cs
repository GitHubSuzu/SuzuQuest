//MenuItem.cs MenuItemコンポーネント
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuItem : MonoBehaviour
{
    public GameObject CursorObj;
    public Text TextObj;
    [SerializeField] bool _isSelecting;

    public enum Kind
    {
        NextMenu,
        BackMenu,
        Event,
    }
    public Kind CurrentKind = Kind.NextMenu;

    public MenuRoot MoveTargetObj;
    public UnityEvent Callbacks;

    public bool IsSelecting
    {
        get => _isSelecting;
        set
        {
            _isSelecting = value;
            CursorObj.SetActive(value);
        }
    }

    public string Text
    {
        get => TextObj.text;
        set
        {
            TextObj.text = value;
        }
    }

    private void Awake()
    {
        CursorObj.SetActive(IsSelecting);
    }
}
