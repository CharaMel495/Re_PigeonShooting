using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.ProbeAdjustmentVolume;

/// <summary>
/// 一定時間ごとに起きるウェーブイベントを表すクラス
/// </summary>
public class WaveEvent
{
    public int ID 
    { get; private  set; }
    
    public float Duration 
    { get; private set; }

    public int EnemyID 
    { get; private set; }
    
    public int SpawnValue 
    { get; private set; }
    
    public float Interval 
    { get; private set; }
    
    public float Distance 
    { get; private set; }
    
    public float AngleRange
    { get; private set; }
    
    public float HPScale 
    { get; private set; }
    
    public float SpeedScale 
    { get; private set; }

    private Action _spawnFunc;

    // コンストラクタ
    public WaveEvent(WaveEventReciever data)
    {
        ID = data.ID;
        Duration = data.Duration;
        EnemyID = data.EnemyID;
        SpawnValue = data.SpawnValue;
        Interval = data.Interval;
        Distance = data.Distance;
        AngleRange = data.AngleRange;
        HPScale = data.HPScale;
        SpeedScale = data.SpeedScale;

        _spawnFunc = DesideFunc(data.SpawnShape);
        
        // 文字列からどの形で敵を出現させるかを決める
        Action DesideFunc(string spawnShape)
        {
            return spawnShape switch
            {
                "Circle" => SpawnCircle,
                "Random" => SpawnRandom,
                _ => () =>
                {
                    Debug.LogWarning($"[WaveEvent] Unknown SpawnShape: '{spawnShape}' (ID={data.ID})");
                }
            };
        }
    }

    // 実行処理
    public void Execute()
    {
        if (EnemyID < 0)
            EventDispatcher.Instance.Dispatch("BossEvent");
        else
            _spawnFunc?.Invoke();
    }

    // 円形に出現させる
    private void SpawnCircle()
    {
        float angleStart = 0f;
        float angleSpan = AngleRange / SpawnValue;
        var playerPos = PlayerManager.Instance.Player.GetPosition();
        for (int i = 0; i < SpawnValue; ++i)
        {
            float angle = angleStart + angleSpan * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 spawnPos = playerPos;
            spawnPos.x += Mathf.Cos(rad) * Distance;
            spawnPos.y += Mathf.Sin(rad) * Distance;

            var offset = new Vector3
            {
                x = Mathf.Cos(rad) * Distance,
                y = Mathf.Sin(rad) * Distance
            };

            var scaler = new EnemyDataStructs.EnemyStatusScaler
            {
                HPScale = HPScale,
                SpeedScale = SpeedScale
            };

            // ファクトリメソッドで敵を出す
            EnemyManager.Instance.CreateEnemy(EnemyID, spawnPos, scaler);
        }
    }

    private void SpawnRandom()
    {
        var playerPos = PlayerManager.Instance.Player.GetPosition();

        for (int i = 0; i < SpawnValue; ++i)
        {
            float angle = UnityEngine.Random.Range(0f, AngleRange);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 spawnPos = playerPos + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad)) * Distance;

            var scaler = new EnemyDataStructs.EnemyStatusScaler
            {
                HPScale = HPScale,
                SpeedScale = SpeedScale
            };

            EnemyManager.Instance.CreateEnemy(EnemyID, spawnPos, scaler);
        }
    }
}
