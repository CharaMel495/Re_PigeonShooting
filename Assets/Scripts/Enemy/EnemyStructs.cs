using System;
using UnityEngine;

namespace EnemyDataStructs
{
    // 敵の速度やHPに補正をかけるようの構造体
    public struct EnemyStatusScaler
    {
        public float HPScale;
        public float SpeedScale;
    }

    /// <summary>
    /// 敵の移動データインターフェース
    /// </summary>
    public interface IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
    }

    /// <summary>
    /// 移動を行わせないようにする為の構造体
    /// </summary>
    public struct NoMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
    }

    /// <summary>
    /// 普通に直進してくる敵
    /// </summary>
    public struct StrainghtNormalMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
    }

    /// <summary>
    /// 指定ポイントに行ったら停止する敵
    /// </summary>
    public struct StopPointMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public Vector3 TargetPoint { get; set; }
        public float StopThreshold { get; set; }
        public IEnemyMoveData NextMove { get; set; }
    }

    /// <summary>
    /// 渦を巻くように動く敵
    /// </summary>
    public struct SpiralMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public float SpiralRatio { get; set; }
    }

    /// <summary>
    /// 渦を巻くように動く敵
    /// ただし中心となるTransformに隷属し、SinとCosから座標を決定する
    /// </summary>
    public struct SlavedSpiralMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public float Distance { get; set; }
        public float AddtionalTime { get; set; }
        public bool IsRightSpin { get; set; }
    }

    /// <summary>
    /// プレイヤーにミサイルのように突っ込んでいく敵
    /// </summary>
    public struct MissileMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float SecondMoveSpeed { get; set; }
        public float MaxMoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float DisAcceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public float TurnRate { get; set; }
        public ITargetProvider Target { get; set; }
        public bool IsStraight { get; set; }
    }

    public struct TrackPlayer : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public float TurnRate { get; set; }
        public ITargetProvider Target { get; set; }
    }

    /// <summary>
    /// 敵の行動情報をまとめたクラス
    /// </summary>
    public interface IEnemyActionData
    {
        public float ActionInterval { get; set; }
        public float CurrentInterval { get; set; }
        public ITargetProvider Target { get; set; }
        public BulletStructs.IBulletCreateData BulletData { get; set; }
    }

    public struct NoAction : IEnemyActionData
    {
        public float ActionInterval { get; set; }
        public float CurrentInterval { get; set; }
        public ITargetProvider Target { get; set; }
        public BulletStructs.IBulletCreateData BulletData { get; set; }
    }

    /// <summary>
    /// 通常の敵行動
    /// </summary>
    public struct SimpleAction : IEnemyActionData
    {
        public float ActionInterval { get; set; }
        public float CurrentInterval { get; set; }
        public ITargetProvider Target { get; set; }
        public BulletStructs.IBulletCreateData BulletData { get; set; }
    }

    /// <summary>
    /// 敵を召喚する敵行動
    /// </summary>
    public struct SummonEnemy : IEnemyActionData
    {
        public float ActionInterval { get; set; }
        public float BulletInterval { get; set; }
        public float CurrentInterval { get; set; }
        public float CurrentBulletInterval { get; set; }
        public ITargetProvider Target { get; set; }
        public BulletStructs.IBulletCreateData BulletData { get; set; }
        public EnemyEnums.EnemyID SummonID { get; set; }
    }

    /// <summary>
    /// 敵の生成情報インターフェース
    /// </summary>
    public interface IEnemyParam
    {
        public int Life { get; set; }
        public int Score { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 BulletSize { get; set; }
        public SpriteData.SpriteType SpriteType { get; }
        public IEnemyMoveData MoveData { get; }
        public IEnemyActionData ActionData { get; }
        public ColliderCategory ColliderCategory { get; }
    }

    /// <summary>
    /// 通常敵
    /// </summary>
    public struct NormalEnemyParam : IEnemyParam
    {
        public int Life { get; set; }
        public int Score { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 BulletSize { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public IEnemyMoveData MoveData { get; set; }
        public IEnemyActionData ActionData { get; set; }
        public ColliderType ColliderType { get; set; }
        public ColliderCategory ColliderCategory { get; }
    }

    /// <summary>
    /// ボス敵
    /// </summary>
    public struct BossEnemyParam : IEnemyParam
    {
        public int Life { get; set; }
        public int Score { get; set; }
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 BulletSize { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public IEnemyMoveData MoveData { get; set; }
        public IEnemyActionData ActionData { get; set; }
        public ColliderCategory ColliderCategory { get; }
    }
}