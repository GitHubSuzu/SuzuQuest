// RandomEncount.cs RandomEncountクラスの実装
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Random Encount")]
public class RandomEncount : ScriptableObject
{
    [System.Serializable]
    public class Data //Mapに出現させたいEnemyのData
    {
        [Range(0, 1)] public float AppearEncountRate;//出現するEnemyの種類の確率
        public EncounterEnemies EncounterEnemies;//出現するEnemyの種類
    }

    [Range(0, 1)] public float MapEncountRate = 0.2f;//Map上のEnemyの出現確率
    public List<Data> List;//Mapに出現させたいEnemyのData(class Data : 種類や確率)を格納するList

    public EncounterEnemies Encount(System.Random rnd)//Enemyに遭遇するか
    {
        //ランダムな浮動小数点がMapEncountRateよりも大きかったらreturn nullを返す
        if (MapEncountRate < rnd.NextDouble()) return null;
        foreach(var d in List)//Listに格納されているEnemyの数だけ回り、dに格納
        {
            var t = rnd.NextDouble();//ランダムな浮動小数点
            if(t < d.AppearEncountRate)//dに格納されているAppearEncountRateがtより大きいか判定
            {
                return d.EncounterEnemies;//dに格納されているEncounterEnemiesを返す
            }
        }
        return null;
    }
}