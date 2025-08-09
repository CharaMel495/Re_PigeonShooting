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
                    var moveData = (BulletStructs.StraightAndAimingMove)bullet.MoveData;

                    // 現在の座標を取得
                    var pos = bullet.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    bullet.transform.position = pos;

                    // 移動速度を更新
                    moveData.MoveSpeed += 
                        (moveData.IsStraight ? moveData.Acceleration : -moveData.DisAcceleration) * Time.fixedDeltaTime;

                    if (moveData.MoveSpeed < 0.0f)
                    {
                        moveData.MoveSpeed = 1.0f;
                        moveData.IsStraight = true;
                        if (moveData.Target != null)
                            moveData.MoveDir = moveData.Target.GetPosition() - bullet.transform.position;
                        else
                        {
                            moveData.MoveDir = (PlayerManager.Instance.Player.GetPosition() - pos).normalized;
                            moveData.MoveSpeed = moveData.SecondMoveSpeed;
                        }
                    }

                    bullet.MoveData = moveData;
                }
                break;
        }
    }
}
