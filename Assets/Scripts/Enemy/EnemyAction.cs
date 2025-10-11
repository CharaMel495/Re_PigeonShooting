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
                        

                        if (moveData.NextMove is EnemyDataStructs.StopPointMove next)
                        {
                            moveData.TargetPoint = StageManager.Instance.GetRandomPositionInArea();
                        }

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
                    var moveData = (EnemyDataStructs.SpiralMove)enemy.MoveData;

                    var lastPos = enemy.transform.position;

                    float spinDir = (moveData.IsRightSpin ? 1f : -1f);

                    // 角度を進める
                    moveData.AngleRad += moveData.MoveSpeed * spinDir * Time.fixedDeltaTime;

                    // 中心点からのオフセット計算（自己中心なのでワールド座標）
                    Vector3 offset = new Vector3(
                        Mathf.Cos(moveData.AngleRad),
                        Mathf.Sin(moveData.AngleRad),
                        0f
                    ) * moveData.Distance;

                    // 新しい位置を反映
                    Vector3 newPos = moveData.Origin + offset;
                    enemy.MoveData.MoveDir = (newPos - enemy.transform.position).normalized;
                    enemy.transform.position = newPos;

                    moveData.MoveDir = (newPos - lastPos).normalized;

                    // 回転演出
                    var euler = enemy.transform.localEulerAngles;
                    euler.z += moveData.MoveSpeed * Time.fixedDeltaTime * 3.0f;
                    if (euler.z > 180f) euler.z -= 360f;
                    if (euler.z < -180f) euler.z += 360f;
                    enemy.transform.localEulerAngles = euler;

                    // MoveDataの更新（忘れずに！）
                    enemy.MoveData = moveData;
                }
                break;


            case EnemyDataStructs.SlavedSpiralMove:
                {
                    var moveData = (EnemyDataStructs.SlavedSpiralMove)enemy.MoveData;

                    var lastPos = enemy.transform.position;

                    float spinDir = (moveData.IsRightSpin ? 1f : -1f);

                    // 角度を加算更新（ここが肝）
                    moveData.AngleRad += moveData.MoveSpeed * spinDir * Time.fixedDeltaTime;

                    // 位置更新
                    var pos = enemy.transform.localPosition;
                    pos.x = Mathf.Cos(moveData.AngleRad) * moveData.Distance;
                    pos.y = Mathf.Sin(moveData.AngleRad) * moveData.Distance;
                    moveData.MoveDir = (pos - enemy.transform.localPosition).normalized;
                    enemy.transform.localPosition = pos;

                    // 回転演出（任意）
                    var euler = enemy.transform.localEulerAngles;
                    euler.z += moveData.MoveSpeed * Time.fixedDeltaTime * 3.0f;
                    if (euler.z > 180f) euler.z -= 360f;
                    if (euler.z < -180f) euler.z += 360f;
                    enemy.transform.localEulerAngles = euler;

                    // MoveDataを戻す（構造体の場合忘れずに！）
                    enemy.MoveData = moveData;
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
                        Vector3 dir = (moveData.Target.GetPosition() - enemy.transform.position).normalized;
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

                    Vector3 dir = (moveData.Target.GetPosition() - enemy.transform.position).normalized;
                  
                    // 現在の移動速度を経過時間から計算
                    moveData.MoveSpeed = moveData.MoveSpeed + moveData.Acceleration * moveData.ElaspedTime;

                    // 移動（前方向へ）
                    enemy.transform.position += dir * moveData.MoveSpeed * Time.fixedDeltaTime;

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
                        data.BulletData.Dir = (data.Target.GetPosition() - enemy.transform.position).normalized;

                    if (enemy.MoveData is EnemyDataStructs.SpiralMove)
                        data.BulletData.Dir = -((EnemyDataStructs.SpiralMove)enemy.MoveData).MoveDir;

                    if (enemy.MoveData is EnemyDataStructs.SlavedSpiralMove)
                        data.BulletData.Dir = -((EnemyDataStructs.SlavedSpiralMove)enemy.MoveData).MoveDir;

                    if (enemy.MoveData is EnemyDataStructs.MissileMove)
                        data.BulletData.Dir = -enemy.transform.up;

                    data.BulletData.Origin = enemy.transform.position;

                    if (data.BulletData is BulletStructs.NoBullet)
                        return;

                    shooter.Shoot(data.BulletData);
                }
                break;

            case EnemyDataStructs.SummonEnemy:
                {
                    EnemyDataStructs.SummonEnemy data = (EnemyDataStructs.SummonEnemy)enemy.ActionData;

                    if (data.CurrentBulletInterval > 0)
                    {
                        if (!(data.BulletData is BulletStructs.NoBullet))
                            shooter.Shoot(data.BulletData);

                        data.CurrentBulletInterval = data.BulletInterval;
                    }
                    else
                        data.CurrentBulletInterval -= Time.fixedDeltaTime;

                    if (data.CurrentInterval > 0)
                    {
                        data.CurrentInterval -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        data.CurrentInterval = data.ActionInterval;
                        SpawnEnemy();
                    }

                    enemy.ActionData = data;

                    return;

                    void SpawnEnemy()
                    {
                        if (data.Target != null)
                            data.BulletData.Dir = (data.Target.GetPosition() - enemy.transform.position).normalized;

                        if (data.SummonID == EnemyEnums.EnemyID.渦巻ぐるぐる敵 ||
                            data.SummonID == EnemyEnums.EnemyID.渦巻ぐるぐる敵_自機狙い単発弾 ||
                            data.SummonID == EnemyEnums.EnemyID.渦巻ぐるぐる敵_後方3way)
                            EnemyManager.Instance.CreateSpiralBarrierEnemy(
                                (int)data.SummonID,
                                enemy.transform.position,
                                5,
                                1.0f,
                                isRightSpiral: true,
                                enemy.transform);

                        if (data.SummonID == EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵 ||
                            data.SummonID == EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵_弾あり)
                            EnemyManager.Instance.CreateEnemy((int)data.SummonID, enemy.transform.position, -enemy.MoveData.MoveDir);
                    }
                }
                break;
        }
    }

    public void Move(Boss boss)
    {
        switch (boss.MoveData)
        {
            case EnemyDataStructs.TrackPlayer:
                {
                    // 移動データを取得
                    var moveData = (EnemyDataStructs.TrackPlayer)boss.MoveData;

                    Vector3 dir = (moveData.Target.GetPosition() - boss.transform.position).normalized;

                    // 現在の移動速度を経過時間から計算
                    moveData.MoveSpeed = moveData.MoveSpeed + moveData.Acceleration * moveData.ElaspedTime;

                    // 移動（前方向へ）
                    boss.transform.position += dir * moveData.MoveSpeed * Time.fixedDeltaTime;

                    boss.MoveData = moveData;
                }
                break;
        }
    }
}
