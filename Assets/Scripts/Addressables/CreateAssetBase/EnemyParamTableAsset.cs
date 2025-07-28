using System.Collections.Generic;
using UnityEngine;

public class EnemyParamTableAsset : ScriptableObject
{
    public List<EnemyParamReciever> EnemyTable;
}

[System.Serializable]
public class EnemyParamTableReciever
{
    public List<EnemyParamReciever> EnemyTable;
}

[System.Serializable]
public class EnemyParamReciever
{
    public int ID;
    public float Size;
    public float BulletSize;
    public int SpriteID;
    public int ColliderType;
    public int Life;
    public int Score;
}
