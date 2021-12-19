//Boss.cs Bossコンポーネントの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : CharacterBase
{
    public void Kill()
    {
        Object.Destroy(gameObject);
    }
}