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
        }
    }
}
