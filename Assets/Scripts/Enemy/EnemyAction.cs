using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyAction
{
    public void Move(Enemy enemy)
    {
        switch (enemy.MoveData)
        {
            case EnemyDataStructs.StrainghtNormalMove:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.StrainghtNormalMove)enemy.MoveData;
                    // 現在の座標を取得
                    var pos = enemy.transform.position;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    enemy.transform.position = pos;
                }
                break;

            case EnemyDataStructs.StopPointMove:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.StopPointMove)enemy.MoveData;

                    // 目的地への距離を計算
                    Vector3 direction = moveData.TargetPoint - enemy.transform.position;
                    float distance = direction.sqrMagnitude;

                    // もし十分に近づいたら
                    if (distance < Mathf.Pow(moveData.StopThreshold, 2))
                    {
                        // 誤差を修正
                        enemy.transform.position = moveData.TargetPoint;
                        // 移動情報を次のものに上書き
                        enemy.MoveData = moveData.NextMove;
                        return;
                    }

                    // 現在の座標を取得
                    var pos = enemy.transform.position;
                    // 移動方向を更新
                    moveData.MoveDir = direction.normalized;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    enemy.transform.position = pos;
                }
                break;

            case EnemyDataStructs.SpiralMove:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.SpiralMove)enemy.MoveData;
                    // 現在の座標を取得
                    var pos = enemy.transform.position;
                    var euler = enemy.transform.localEulerAngles;
                    moveData.MoveDir = Quaternion.AngleAxis
                        (moveData.SpiralRatio * Time.fixedDeltaTime, Vector3.forward) * moveData.MoveDir;
                    moveData.MoveSpeed += moveData.ElaspedTime * moveData.Acceleration;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // それっぽく見せる為に回転もかける
                    euler.z += moveData.SpiralRatio * Time.fixedDeltaTime * 3.0f;
                    if (euler.z > 180)
                        euler.z -= 360;
                    if (euler.z < -180)
                        euler.z += 360;
                    enemy.transform.localEulerAngles = euler;
                    // 座標を更新
                    enemy.transform.position = pos;
                    enemy.MoveData.MoveDir = moveData.MoveDir;
                }
                break;

            case EnemyDataStructs.SlavedSpiralMove:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.SlavedSpiralMove)enemy.MoveData;
                    float spinDir = moveData.IsRightSpin ? 1 : -1;
                    // 現在の座標を取得
                    var pos = enemy.transform.localPosition;
                    var euler = enemy.transform.localEulerAngles;
                    moveData.MoveDir = Quaternion.AngleAxis
                        (moveData.MoveSpeed * Time.fixedDeltaTime, Vector3.forward) * moveData.MoveDir;
                    moveData.MoveSpeed += moveData.ElaspedTime * moveData.Acceleration;
                    // 移動後座標を計算
                    var time = Time.fixedTime * spinDir;
                    var addtionalTime = moveData.AddtionalTime * spinDir;
                    pos.x = Mathf.Cos(time + addtionalTime) * moveData.Distance;
                    pos.y = Mathf.Sin(time + addtionalTime) * moveData.Distance;
                    // それっぽく見せる為に回転もかける
                    euler.z += moveData.MoveSpeed * Time.fixedDeltaTime * 3.0f;
                    if (euler.z > 180)
                        euler.z -= 360;
                    if (euler.z < -180)
                        euler.z += 360;
                    enemy.transform.localEulerAngles = euler;
                    // 座標を更新
                    enemy.transform.localPosition = pos;
                    enemy.MoveData.MoveDir = moveData.MoveDir;
                }
                break;

            case EnemyDataStructs.MissileMove:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.MissileMove)enemy.MoveData;

                    if (moveData.IsStraight)
                    {
                        // 現在の移動速度を経過時間から計算
                        moveData.SecondMoveSpeed += moveData.Acceleration * moveData.ElaspedTime;

                        // 最高速を超えないように
                        moveData.SecondMoveSpeed = Mathf.Min(moveData.SecondMoveSpeed, moveData.MaxMoveSpeed);

                        // 移動（前方向へ）
                        enemy.transform.position += enemy.transform.up *
                            moveData.SecondMoveSpeed * Time.fixedDeltaTime;
                    }
                    else
                    {
                        Vector3 dir = (moveData.Target.GetPostion() - enemy.transform.position).normalized;
                        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, dir);

                        // 徐々に向く
                        enemy.transform.rotation = Quaternion.RotateTowards(
                            enemy.transform.rotation,
                            targetRotation,
                            moveData.TurnRate * Time.fixedDeltaTime
                        );

                        // 現在の移動速度を経過時間から計算
                        moveData.MoveSpeed += moveData.DisAcceleration * Time.fixedDeltaTime;

                        // 移動（前方向へ）
                        enemy.transform.position += enemy.transform.up *
                            moveData.MoveSpeed * Time.fixedDeltaTime;

                        // 速度が負の値にまで落ちたら直進に切替
                        moveData.IsStraight = enemy.MoveData.MoveSpeed < 0.0f;

                        // 直進に切り替わったら経過時間をリセット
                        if (moveData.IsStraight)
                            moveData.ElaspedTime = 0.0f;
                    }

                    enemy.MoveData = moveData;
                }
                break;

            case EnemyDataStructs.TrackPlayer:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.TrackPlayer)enemy.MoveData;

                    Vector3 dir = (moveData.Target.GetPostion() - enemy.transform.position).normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, dir);

                    // 徐々に向く
                    enemy.transform.rotation = Quaternion.RotateTowards(
                        enemy.transform.rotation,
                        targetRotation,
                        moveData.TurnRate * Time.fixedDeltaTime
                    );

                    // 現在の移動速度を経過時間から計算
                    moveData.MoveSpeed += moveData.Acceleration * Time.fixedDeltaTime;

                    // 移動（前方向へ）
                    enemy.transform.position += enemy.transform.up *
                        moveData.MoveSpeed * Time.fixedDeltaTime;

                    enemy.MoveData = moveData;
                }
                break;
        }
    }

    public void Action(Enemy enemy, BulletShooter shooter)
    {
        if (enemy.ActionData == null)
            return;

        switch (enemy.ActionData)
        {
            case EnemyDataStructs.SimpleAction:
                {
                    EnemyDataStructs.SimpleAction data = (EnemyDataStructs.SimpleAction)enemy.ActionData;

                    if (data.CurrentInterval > 0)
                    {
                        enemy.AdvanceInterval();
                        return;
                    }
                    else
                        enemy.SetActionInterval();

                    if (data.Target != null)
                        data.BulletData.Dir = (data.Target.GetPostion() - enemy.transform.position).normalized;

                    if (enemy.MoveData is EnemyDataStructs.SpiralMove)
                        data.BulletData.Dir = -((EnemyDataStructs.SpiralMove)enemy.MoveData).MoveDir;

                    if (enemy.MoveData is EnemyDataStructs.MissileMove)
                        data.BulletData.Dir = -enemy.transform.up;

                    shooter.Shoot(data.BulletData);
                }
                break;
        }
    }
}
