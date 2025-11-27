using UnityEngine;

public class BulletMover
{
    public void MoveBullet(Bullet bullet)
    {
        switch (bullet.MoveData)
        {
            case BulletStructs.StraightMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.StraightMove)bullet.MoveData;
                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    bullet.MoveData.MoveSpeed += bullet.MoveData.Acceleration * Time.fixedDeltaTime;
                }
                break;

            case BulletStructs.StraightAndAimingMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.CurveAndStraightMove)bullet.MoveData;

                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    // 移動速度を更新
                    moveData.MoveSpeed +=
                        (moveData.IsStraight ? moveData.Acceleration : -moveData.DisAcceleration) * Time.fixedDeltaTime;

                    if (!moveData.IsStraight)
                    {
                        var addValue = Time.fixedDeltaTime * moveData.AngleSpeed;
                        moveData.MoveDir = Quaternion.Euler(0, 0, addValue) * moveData.MoveDir;
                        moveData.CurvedAngle += addValue;
                    }

                    if (moveData.MoveSpeed < 0.0f || moveData.CurvedAngle > moveData.CurveAngle)
                    {
                        moveData.IsStraight = false;
                        moveData.MoveSpeed = moveData.SecondMoveSpeed;
                    }
                }
                break;

            case BulletStructs.CurveAndStraightMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.CurveAndStraightMove)bullet.MoveData;

                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    // 移動速度を更新
                    moveData.MoveSpeed +=
                        (moveData.IsStraight ? moveData.Acceleration : -moveData.DisAcceleration) * Time.fixedDeltaTime;

                    if (!moveData.IsStraight)
                    {
                        var addValue = Time.fixedDeltaTime * moveData.AngleSpeed;
                        moveData.MoveDir = Quaternion.Euler(0, 0, addValue) * moveData.MoveDir;
                        moveData.CurvedAngle += addValue;
                    }

                    if (moveData.MoveSpeed < 0.0f || moveData.CurvedAngle > moveData.CurveAngle)
                    {
                        moveData.IsStraight = true;
                        moveData.MoveSpeed = moveData.SecondMoveSpeed;
                    }

                    bullet.MoveData = moveData;
                }
                break;

            case BulletStructs.StraightAndCurveMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.StraightAndCurveMove)bullet.MoveData;

                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    // 移動速度を更新
                    moveData.MoveSpeed +=
                        (moveData.IsStraight ? -moveData.DisAcceleration : moveData.Acceleration) * Time.fixedDeltaTime;

                    if (!moveData.IsStraight)
                    {
                        var addValue = Time.fixedDeltaTime * moveData.AngleSpeed;
                        moveData.MoveDir = Quaternion.Euler(0, 0, addValue) * moveData.MoveDir;
                        moveData.CurvedAngle += addValue;
                    }
                    else if (moveData.MoveSpeed < 0.0f || moveData.CurvedAngle > moveData.CurveAngle)
                    {
                        moveData.IsStraight = false;
                        moveData.MoveSpeed = moveData.SecondMoveSpeed;
                    }

                    bullet.MoveData = moveData;
                }
                break;

            case BulletStructs.CurveMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.CurveMove)bullet.MoveData;
                    var addValue = Time.fixedDeltaTime * moveData.AngleSpeed;
                    moveData.MoveDir = Quaternion.Euler(0, 0, addValue) * moveData.MoveDir;
                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    moveData.MoveSpeed += bullet.MoveData.Acceleration * Time.fixedDeltaTime;

                    bullet.MoveData = moveData;
                }
                break;

            case BulletStructs.BounceMove:
                {
                    // 移動データを取得
                    var moveData = (BulletStructs.BounceMove)bullet.MoveData;
                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    bullet.MoveData.MoveSpeed += bullet.MoveData.Acceleration * Time.fixedDeltaTime;

                    if (!bullet.IsInCamera && moveData.BounceCount < moveData.BounceLimit)
                        Bounce();

                    bullet.MoveData = moveData;

                    void Bounce()
                    {
                        var screenPos = Camera.main.WorldToScreenPoint(bullet.transform.position);

                        if (screenPos.x < 0 || screenPos.x > Screen.width)
                        {
                            screenPos.x
                                = Mathf.Max(0, Mathf.Min(screenPos.x, Screen.width));

                            var dir = moveData.MoveDir;
                            dir.x *= -1;
                            moveData.MoveDir = dir;

                            ++moveData.BounceCount;

                            bullet.transform.position = Camera.main.ScreenToWorldPoint(screenPos);
                        }

                        if (screenPos.y < 0 || screenPos.y > Screen.height)
                        {
                            screenPos.y
                                = Mathf.Max(0, Mathf.Min(screenPos.y, Screen.height));

                            var dir = moveData.MoveDir;
                            dir.y *= -1;
                            moveData.MoveDir = dir;

                            ++moveData.BounceCount;

                            bullet.transform.position = Camera.main.ScreenToWorldPoint(screenPos);
                        }
                    }
                }
                break;
        }
    }
}
