using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrowStatus
{
    public enum GrowStatus
    {
        ShotValue,
        ShotPower,
        MinShotRate,
        MaxShotRate,
        InvincibleTime,
        DashTime
    }

    // 一回の射撃で発射する弾の数
    public int ShotValue
    { get; private set; }

    // 弾一発の攻撃力
    public int ShotPower
    { get; private set; }

    // 弾の発射間隔
    public float MinShotRate
    { get; private set; }
    public float MaxShotRate
    { get; private set; }
    // 被弾後無敵時間
    public float InvincibleTime
    { get; private set; }
    // ダッシュ継続時間
    public float DashTime
    { get; private set; }

    // 成長時どれだけ成長するか
    private readonly int _shotValueGrowValue = 1;
    private readonly int _shotPowerGrowValue = 1;
    private readonly float _maxShotRateGrowValue = 0.05f;
    private readonly float _minShotRateGrowValue = 0.05f;
    private readonly float _invincibleTimeGrowValue = 0.1f;
    private readonly float _dashTimeGrowValue = 0.1f;

    // 成長限界
    private readonly int _maxShotValue = 10;
    private readonly int _maxShotPower = 5;
    private readonly float _maxShotRateGrowLimit = 0.3f;
    private readonly float _minShotRateGrowLimit = 0.03f;
    private readonly float _maxInvincibleTime = 0.1f;
    private readonly float _maxDashTime = 0.1f;

    public int Level
    { get; set; }

    private int _exp;
    public int EXP
    {
        get => _exp;

        set
        {
            _exp = value;

            // もし経験値ボーダーに達していたら
            if (_exp < _nextBoarder)
                return;

            LevelUp();   
        }
    }

    // 次のレベルまでのボーダー
    private int _nextBoarder = 10;

    private Dictionary<GrowStatus, Action> _statusGrown;
    private GrowStatus[] _growOrder;
    private int _growIdx;

    public PlayerGrowStatus(PlayerDefaultStatus defaultStatus)
    {
        ShotValue = defaultStatus.ShotValue;
        ShotPower = defaultStatus.ShotPower;
        MinShotRate = defaultStatus.MinShotRate;
        MaxShotRate = defaultStatus.MaxShotRate;
        InvincibleTime = defaultStatus.InvincibleTime;
        DashTime = defaultStatus.DashTime;
        _growOrder = defaultStatus.GrowOrder;

        _exp = 0;
        _growIdx = 0;

        _nextBoarder = 5;

        _statusGrown = new Dictionary<GrowStatus, Action>
        {
            { GrowStatus.ShotValue, () => ShotValue = Mathf.Min(ShotValue + _shotValueGrowValue, _maxShotValue) },
            { GrowStatus.ShotPower, () => ShotPower = Mathf.Min(ShotPower + _shotPowerGrowValue, _maxShotPower) },
            { GrowStatus.MinShotRate, () => MinShotRate = Mathf.Max(MinShotRate - _minShotRateGrowValue, _minShotRateGrowLimit) },
            { GrowStatus.MaxShotRate, () => MaxShotRate = Mathf.Max(MaxShotRate - _maxShotRateGrowValue, _maxShotRateGrowLimit)  },
            { GrowStatus.InvincibleTime, () => InvincibleTime = Mathf.Min(InvincibleTime + _invincibleTimeGrowValue, _maxInvincibleTime) },
            { GrowStatus.DashTime, () => DashTime = Mathf.Min(DashTime + _dashTimeGrowValue, _maxDashTime)  },
        };
    }

    private void LevelUp()
    {
        // 経験値をリセット
        _exp = 0;
        // 代わりにレベルをひとつ上げる
        ++Level;

        // 能力を成長させる
        _statusGrown[_growOrder[_growIdx]].Invoke();

        // プレイヤーの弾のパラメータ更新イベント呼び出し
        EventDispatcher.Instance.Dispatch("PlayerUpdateBulletParameter");

        // 次の成長を予約
        ++_growIdx;
        _growIdx %= _growOrder.Length;
    }
}
