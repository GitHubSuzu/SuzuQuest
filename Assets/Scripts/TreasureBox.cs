using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSaveData = SaveData;

public class TreasureBox : CharacterBase
{
    [SerializeField] bool _doOpen;
    public bool DoOpen
    {
        get => !IsActive;
        set
        {
            IsActive = !value;
        }
    }
    public void Open()
    {
        DoOpen = true;
    }

    public class TreasureSaveData : SaveData
    {
        public bool DoOpen;
        public TreasureSaveData(TreasureBox self) : base(self)
        {
            DoOpen = self.DoOpen;
        }
    }

    public override SaveData GetSaveData()
    {
        return new TreasureSaveData(this);
    }

    public override void LoadSaveData(string saveDataJson)
    {
        base.LoadSaveData(saveDataJson);

        var saveData = JsonUtility.FromJson<TreasureSaveData>(saveDataJson);
        if (saveData == null) return;

        DoOpen = saveData.DoOpen;
    }

}