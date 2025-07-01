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
                        // 移動情報を移動禁止に
                        enemy.MoveData = new EnemyDataStructs.NoMove();
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
                    moveData.MoveDir = Quaternion.AngleAxis
                        (moveData.SpiralRatio * Time.fixedDeltaTime, Vector3.forward) * moveData.MoveDir;
                    moveData.MoveSpeed += moveData.ElaspedTime * moveData.Acceleration;
                    // 移動後座標を計算
                    pos += moveData.MoveDir * moveData.MoveSpeed * Time.fixedDeltaTime;
                    // 座標を更新
                    enemy.transform.position = pos;
                    enemy.MoveData.MoveDir = moveData.MoveDir;
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

                    shooter.Shoot(data.BulletData);
                }
                break;
        }
    }
}
