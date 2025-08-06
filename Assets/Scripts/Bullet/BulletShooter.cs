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

            case BulletStructs.StraightAimingShoot:

                BulletManager.Instance.CreateBullet((BulletStructs.StraightAimingShoot)shootData);

                break;

            case BulletStructs.SpreadEightAimingShoot:
                {
                    var data = (BulletStructs.SpreadEightAimingShoot)shootData;
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

            case BulletStructs.RingShot:
                {
                    var data = (BulletStructs.RingShot)shootData;
                    // シフト演算でサクッと計算(360 ÷ 8)
                    float angleSpan = (float)(360 >> 3);
                    data.SecondDir = (PlayerManager.Instance.Player.GetPostion() - data.Origin).normalized;

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

    public void Shoot(BulletStructs.IBulletCreateData shootData, float slopeCondition)
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

                    // 発射ポイントの固定配置（前方の円上）
                    float radius = 0.5f;
                    Vector3 basePos = data.Origin + data.Dir.normalized * radius;

                    // 左右のオフセット位置（固定間隔）
                    float horizontalOffset = 0.4f;
                    Vector3 leftPos = basePos + (Quaternion.Euler(0, 0, 90) * data.Dir.normalized) * horizontalOffset;
                    Vector3 rightPos = basePos + (Quaternion.Euler(0, 0, -90) * data.Dir.normalized) * horizontalOffset;

                    // 傾きに応じた発射角度
                    float maxSpread = 90f;
                    float spread = Mathf.Lerp(0f, maxSpread, 1 - slopeCondition);

                    // 中央
                    var centerData = data;
                    centerData.Origin = basePos;
                    centerData.Dir = data.Dir;
                    BulletManager.Instance.CreateBullet(centerData);

                    // 左
                    var leftData = data;
                    leftData.Origin = leftPos;
                    leftData.Dir = Quaternion.AngleAxis(spread, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(leftData);

                    // 右
                    var rightData = data;
                    rightData.Origin = rightPos;
                    rightData.Dir = Quaternion.AngleAxis(-spread, Vector3.forward) * data.Dir;
                    BulletManager.Instance.CreateBullet(rightData);
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

            case BulletStructs.MultiWayShot:
                {
                    var data = (BulletStructs.MultiWayShot)shootData;

                    if (data.ShotValue < 3) data.ShotValue = 3;
                    if (data.ShotValue > 15) data.ShotValue = 15;

                    Vector3 forwardDir = data.Dir.normalized;

                    // 前方円上の基準位置
                    float forwardOffset = 0.0f;
                    Vector3 basePos = data.Origin + forwardDir * forwardOffset;

                    // 最大拡散角度（左右に半分ずつ割る）
                    float maxSpread = 100f; // 最大広がり角（調整可）
                    float spread = Mathf.Lerp(0f, maxSpread, 1 - slopeCondition);

                    // 中央を0度として左右に等間隔配置
                    int half = (data.ShotValue - 1) / 2;
                    float angleStep = (data.ShotValue > 1) ? spread / half : 0f;

                    for (int i = 0; i < data.ShotValue; i++)
                    {
                        // 発射位置（横方向固定間隔）
                        float horizontalOffset = (i - half) * 0.2f; // 横間隔は固定値
                        Vector3 shootPos = basePos + (Quaternion.Euler(0, 0, 90) * forwardDir) * horizontalOffset;

                        // 発射角度（中心からのズレ）
                        float angle = (i - half) * angleStep;

                        var temp = data;
                        temp.Origin = shootPos;
                        temp.Dir = Quaternion.AngleAxis(angle, Vector3.forward) * forwardDir;
                        BulletManager.Instance.CreateBullet(temp);
                    }
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

            case BulletStructs.StraightAimingShoot:

                BulletManager.Instance.CreateBullet((BulletStructs.StraightAimingShoot)shootData);

                break;

            case BulletStructs.SpreadEightAimingShoot:
                {
                    var data = (BulletStructs.SpreadEightAimingShoot)shootData;
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

            case BulletStructs.RingShot:
                {
                    var data = (BulletStructs.RingShot)shootData;
                    // シフト演算でサクッと計算(360 ÷ 8)
                    float angleSpan = (float)(360 >> 3);
                    data.SecondDir = PlayerManager.Instance.Player.GetPostion();

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
