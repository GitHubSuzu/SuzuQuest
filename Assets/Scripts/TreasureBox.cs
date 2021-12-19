//TreasureBox.cs TreasureBoxコンポーネントの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureBox : CharacterBase
{
    public void Open()
    {
        Object.Destroy(gameObject);
    }
}