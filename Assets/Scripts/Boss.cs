using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : CharacterBase
{
    public void Kill()
    {
        IsActive = false;
    }

    public class BossSaveData : SaveData
    {
        public bool IsKill;
        public BossSaveData(Boss self) : base(self)
        {
            IsKill = !self.IsActive;
        }
    }

    public override SaveData GetSaveData()
    {
        return new BossSaveData(this);
    }
    public override void LoadSaveData(string saveDataJson)
    {
        base.LoadSaveData(saveDataJson);

        var saveData = JsonUtility.FromJson<BossSaveData>(saveDataJson);
        if (saveData == null) return;

        IsActive = !saveData.IsKill;
    }

}