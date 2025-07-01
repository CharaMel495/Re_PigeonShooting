using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Strainght_LowSpeed,
    Strainght_NormalSpeed,
    Strainght_HighSpeed,
}

public class BulletDatabase
{
    private Dictionary<BulletType, BulletStructs.IBulletCreateData> _dataDic = new();

    public void Initialize()
    {
        CreateDic();
    }

    private void CreateDic()
    {
        _dataDic.Add(BulletType.Strainght_LowSpeed,
            new BulletStructs.StaraightShoot
            {
                MoveSpeed = 10.0f,
                Dir = -Vector3.right,
                Origin = new(10.0f, 0.0f, 0.0f),
                Scale = Vector3.one,
                SpriteType = SpriteData.SpriteType.PlayerBullet
            });
    }

    public BulletStructs.IBulletCreateData GetBulletData(BulletType key)
    {
        if (!_dataDic.ContainsKey(key))
            return null;

        return _dataDic[key];
    }
}
