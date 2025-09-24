using System.Collections.Generic;
using UnityEngine;

public class WaveEventTableAsset : ScriptableObject
{
    public List<WaveEventReciever> WaveEventTable;
}

[System.Serializable]
public class WaveEventTableReciever
{
    public List<WaveEventReciever> WaveEventTable;
}

[System.Serializable]
public class WaveEventReciever
{
    public int ID;
    public float Duration;
    public int EnemyID;
    public int SpawnValue;
    public float Interval;
    public float Distance;
    public float AngleRange;
    public float HPScale;
    public float SpeedScale;
    public string SpawnShape;
}