using BulletStructs;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Strainght_LowSpeed,
    Strainght_NormalSpeed,
    Strainght_HighSpeed,
}

/// <summary>
/// 弾のパラメータプリセットをデータベースとしてまとめて持っておくクラス
/// </summary>
public class BulletDatabase
{
    private IBulletCreateData[] _dataBase { get; set; }

    private Dictionary<Type, int> _idxDic;

    public void Initialize(BulletParamPreset preset)
    {
        CreateDataBaseFromPreset(preset);
    }

    private void CreateDataBaseFromPreset(BulletParamPreset preset)
    {
        var list = new List<IBulletCreateData>();
        _idxDic = new();

        _idxDic.Add(typeof(StaraightShoot), 0);
        foreach (var item in preset.StraightShoot)
            list.Add(item);
        _idxDic.Add(typeof(TwoWayStraightShoot), list.Count);
        foreach (var item in preset.TwoWayStraightShoot)
            list.Add(item);
        _idxDic.Add(typeof(ThreeWayShoot), list.Count);
        foreach (var item in preset.ThreeWayShoot)
            list.Add(item);
        _idxDic.Add(typeof(FourWayShoot), list.Count);
        foreach (var item in preset.FourWayShoot)
            list.Add(item);
        _idxDic.Add(typeof(MultiWayShot), list.Count);
        foreach (var item in preset.MultiWayShot)
            list.Add(item);
        _idxDic.Add(typeof(SpreadFourShoot), list.Count);
        foreach (var item in preset.SpreadFourShoot)
            list.Add(item);
        _idxDic.Add(typeof(SpreadEightShoot), list.Count);
        foreach (var item in preset.SpreadEightShoot)
            list.Add(item);
        _idxDic.Add(typeof(StraightAimingShoot), list.Count);
        foreach (var item in preset.StraightAimingShoot)
            list.Add(item);
        _idxDic.Add(typeof(SpreadEightAimingShoot), list.Count);
        foreach (var item in preset.SpreadEightAimingShoot)
            list.Add(item);
        _idxDic.Add(typeof(RingShot), list.Count);
        foreach (var item in preset.RingShot)
            list.Add(item);
        _idxDic.Add(typeof(FiveWayAndBackMonoShoot), list.Count);
        foreach (var item in preset.FiveWayAndBackMonoShoot)
            list.Add(item);
        _idxDic.Add(typeof(LazerParam), list.Count);
        foreach (var item in preset.LazerParam)
            list.Add(item);
        _idxDic.Add(typeof(CrossLazerParam), list.Count);
        foreach (var item in preset.CrossLazer)
            list.Add(item);
        _idxDic.Add(typeof(CurveAndStraight), list.Count);
        foreach (var item in preset.CurveAndStraight)
            list.Add(item);
        _idxDic.Add(typeof(StraightAndCurve), list.Count);
        foreach (var item in preset.StraightAndCurve)
            list.Add(item);
        _idxDic.Add(typeof(Curve), list.Count);
        foreach (var item in preset.Curve)
            list.Add(item);
        _idxDic.Add(typeof(MultiCurve), list.Count);
        foreach (var item in preset.MultiCurve)
            list.Add(item);
        _idxDic.Add(typeof(Bounce), list.Count);
        foreach (var item in preset.Bounce)
            list.Add(item);
        _idxDic.Add(typeof(MultiBounce), list.Count);
        foreach (var item in preset.MultiBounce)
            list.Add(item);
        _idxDic.Add(typeof(BounceRing), list.Count);
        foreach (var item in preset.BounceRing)
            list.Add(item);

        _dataBase = list.ToArray();
    }

    public IBulletCreateData GetBulletData(Type type, int idx)
    {
        if (!_idxDic.ContainsKey(type))
            return null;

        return _dataBase[_idxDic[type] + idx];
    }
}