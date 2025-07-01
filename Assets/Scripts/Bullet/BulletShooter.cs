using UnityEngine;

/// <summary>
/// 弾を発射するクラス
/// </summary>
public class BulletShooter
{
    /// <summary>
    /// ここで弾を撃つときに使うトランスフォーム
    /// インスタンスを分けるためここでキャッシュ
    /// </summary>
    private Transform _transformCache;

    public void Shoot(BulletStructs.IBulletCreateData shootData)
    {
        switch (shootData)
        {
            case BulletStructs.StaraightShoot:

                BulletManager.Instance.CreateBullet((BulletStructs.StaraightShoot)shootData);
                break;

            case BulletStructs.TwoWayStraightShoot:
                {
                    var data = (BulletStructs.TwoWayStraightShoot)shootData;
                    var pos = data.Origin;
                    pos.y += data.BulletSpan * 0.5f;
                    data.Origin = pos;
                    BulletManager.Instance.CreateBullet(data);
                    pos.y -= data.BulletSpan;
                    data.Origin = pos;
                    BulletManager.Instance.CreateBullet(data);
                }
                break;

            case BulletStructs.ThreeWayShoot:
                {
                    var data = (BulletStructs.ThreeWayShoot)shootData;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(data.AngleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(data.AngleSpan * -2.0f, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                }
                break;

            case BulletStructs.FourWayShoot:
                {
                    var data = (BulletStructs.FourWayShoot)shootData;
                    data.Dir = Quaternion.AngleAxis(data.AngleSpan * 0.5f, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(data.AngleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(data.AngleSpan * -2.0f, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(-data.AngleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                }
                break;

            case BulletStructs.SpreadEightShoot:
                {
                    var data = (BulletStructs.SpreadEightShoot)shootData;
                    // シフト演算でサクッと計算(360 ÷ 8)
                    float angleSpan = (float)(360 >> 3);

                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                    data.Dir = Quaternion.AngleAxis(angleSpan, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(data);
                }
                break;
        }
    }
}
