using UnityEngine;

namespace EnemyDataStructs
{
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

    public struct StopPointMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public Vector3 TargetPoint { get; set; }
        public float StopThreshold { get; set; }
    }

    public struct SpiralMove : IEnemyMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float ElaspedTime { get; set; }
        public Vector3 MoveDir { get; set; }
        public float SpiralRatio { get; set; }
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