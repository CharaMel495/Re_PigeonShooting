using System.Collections.Generic;
using UnityEngine;

public class WaveTimeTableAsset : ScriptableObject
{
    public List<WaveTimeReciever> WaveTimeTable;
}

[System.Serializable]
public class WaveTimeTableReciever
{
    public List<WaveTimeReciever> WaveTimeTable;
}

[System.Serializable]
public class WaveTimeReciever
{
    public int ID;
    public float StartTime;
    public int EventID;
    public bool IsClearEnemys;
}