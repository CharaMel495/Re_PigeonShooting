using UnityEngine;

namespace BulletStructs
{
    public interface IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
    }

    public struct StraightMove : IBulletMoveData
    {
        public Vector3 MoveDir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
    }

    public interface IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Dir { get; set; }
        public float Acceleration { get; set; }
        public SpriteData.SpriteType SpriteType { get; }
        public IBulletMoveData CreateMoveData();
        public ColliderCategory ColCategory { get; set; }
    }

    /// <summary>
    /// 直進で動く弾用の構造体
    /// </summary>
    public struct StaraightShoot : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; } 
        public Vector3 Dir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public ColliderCategory ColCategory { get; set; }

        public StaraightShoot GetData() => this;
        public IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    public struct TwoWayStraightShoot : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public Vector3 Dir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float BulletSpan { get; set; }
        public ColliderCategory ColCategory { get; set; }

        public TwoWayStraightShoot GetData() => this;
        public IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    /// <summary>
    /// 3方向スプレッド弾
    /// </summary>
    public struct ThreeWayShoot : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public Vector3 Dir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float AngleSpan { get; set; }
        public ColliderCategory ColCategory { get; set; }

        public ThreeWayShoot GetData() => this;
        public IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    /// <summary>
    /// 4方向スプレッド弾
    /// </summary>
    public struct FourWayShoot : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public Vector3 Dir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float AngleSpan { get; set; }
        public ColliderCategory ColCategory { get; set; }

        public FourWayShoot GetData() => this;
        public IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    /// <summary>
    /// 4方向スプレッド弾
    /// </summary>
    public struct SpreadEightShoot : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public Vector3 Dir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public ColliderCategory ColCategory { get; set; }

        public SpreadEightShoot GetData() => this;
        public IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    public struct LazerParam : IBulletCreateData
    {
        public Vector3 Origin { get; set; }
        public Vector3 Scale { get; set; }
        public Vector3 Dir { get; set; }
        public float Acceleration { get; set; }
        public SpriteData.SpriteType SpriteType { get; set; }
        public IBulletMoveData CreateMoveData() => null;
        public ColliderCategory ColCategory { get; set; }
        public float OpenTime { get; set; }
        public float CloseTime { get; set; }
        public float KeepTime { get; set; }
        public float Width { get; set; }
        public float Length { get; set; }
        public float MoveSpeed { get; set; }
        public Vector3 Target { get; set; }
        public float Interval { get; set; }
    }
}
