using UnityEngine;

namespace BulletStructs
{
    public interface IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public int Damage { get; set; }
    }

    public struct StraightMove : IBulletMoveData
    {
        public Vector3 MoveDir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public int Damage { get; set; }
    }

    public struct StraightAndAimingMove : IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float SecondMoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float DisAcceleration { get; set; }
        public Vector3 MoveDir { get; set; }
        public Vector3 SecondDir { get; set; }
        public ITargetProvider Target { get; set; }
        public bool IsStraight { get; set; }
        public int Damage { get; set; }
    }

    public struct CurveAndStraightMove : IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float SecondMoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float DisAcceleration { get; set; }
        public Vector3 MoveDir { get; set; }
        public Vector3 SecondDir { get; set; }
        public bool IsStraight { get; set; }
        public int Damage { get; set; }
        public float CurveAngle { get; set; }
        public float CurvedAngle { get; set; }
        public float AngleSpeed { get; set; }
    }

    public struct StraightAndCurveMove : IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float SecondMoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public float DisAcceleration { get; set; }
        public Vector3 MoveDir { get; set; }
        public Vector3 SecondDir { get; set; }
        public bool IsStraight { get; set; }
        public int Damage { get; set; }
        public float CurveAngle { get; set; }
        public float CurvedAngle { get; set; }
        public float AngleSpeed { get; set; }
    }

    public struct CurveMove : IBulletMoveData
    {
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public Vector3 MoveDir { get; set; }
        public int Damage { get; set; }
        public float AngleSpeed { get; set; }
    }

    public struct BounceMove : IBulletMoveData
    {
        public Vector3 MoveDir { get; set; }
        public float MoveSpeed { get; set; }
        public float Acceleration { get; set; }
        public int Damage { get; set; }
        public int BounceCount { get; set; }
        public int BounceLimit { get; set; }
    }

    public abstract class IBulletCreateData
    {
        [System.NonSerialized]
        public Vector3 Origin;
        public Vector3 Scale;
        [System.NonSerialized]
        public Vector3 Dir;
        public float Acceleration;
        public SpriteData.SpriteType SpriteType;
        public abstract IBulletMoveData CreateMoveData();
        public ColliderCategory ColCategory;
        public int Damage;
    }

    /// <summary>
    /// 直進で動く弾用の構造体
    /// </summary>
    [System.Serializable]
    public class NoBullet : IBulletCreateData
    {
        public override IBulletMoveData CreateMoveData() { return null; }
    }

    /// <summary>
    /// 直進で動く弾用の構造体
    /// </summary>
    [System.Serializable]
    public class StaraightShoot : IBulletCreateData
    {
        public float MoveSpeed;

        public StaraightShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    [System.Serializable]
    public class TwoWayStraightShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float BulletSpan;

        public TwoWayStraightShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
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
    [System.Serializable]
    public class ThreeWayShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpan;

        public ThreeWayShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
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
    [System.Serializable]
    public class FourWayShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpan;

        public FourWayShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration
            };
        }
    }

    [System.Serializable]
    public class MultiWayShot : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpan;
        public int ShotValue;

        public MultiWayShot GetData() => this;
        public override IBulletMoveData CreateMoveData()
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
    /// 8方向スプレッド弾
    /// </summary>
    [System.Serializable]
    public class SpreadEightShoot : IBulletCreateData
    {
        public float MoveSpeed;

        public SpreadEightShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
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
    /// まっすぐ動いた後、何かに向かって進む弾
    /// </summary>
    [System.Serializable]
    public class StraightAimingShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float DisAcceleration;

        public StraightAimingShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightAndAimingMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                DisAcceleration = DisAcceleration,
                Target = PlayerManager.Instance.Player
            };
        }
    }

    /// <summary>
    /// 8方向版まっすぐ動いた後、何かに向かって進む弾
    /// </summary>
    [System.Serializable]
    public class SpreadEightAimingShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float DisAcceleration;

        public SpreadEightAimingShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightAndAimingMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                DisAcceleration = DisAcceleration,
                Target = PlayerManager.Instance.Player
            };
        }
    }

    /// <summary>
    /// 輪っか弾
    /// </summary>
    [System.Serializable]
    public class RingShot : IBulletCreateData
    {
        public Vector3 SecondDir;
        public float MoveSpeed;
        public float SecondMoveSpeed;
        public float DisAcceleration;

        public RingShot GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightAndAimingMove
            {
                MoveDir = Dir,
                SecondMoveSpeed = SecondMoveSpeed,
                SecondDir = SecondDir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                DisAcceleration = DisAcceleration
            };
        }
    }

    /// <summary>
    /// 5方向スプレッド弾+後方単発
    /// </summary>
    [System.Serializable]
    public class FiveWayAndBackMonoShoot : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpan;

        public FiveWayAndBackMonoShoot GetData() => this;
        public override IBulletMoveData CreateMoveData()
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
    /// レーザー弾
    /// </summary>
    [System.Serializable]
    public class LazerParam : IBulletCreateData
    {
        public override IBulletMoveData CreateMoveData() => null;
        public float OpenTime;
        public float CloseTime;
        public float KeepTime;
        public float Width;
        public float Length;
        public float MoveSpeed;
        public Vector3 Target;
        public float Interval;
    }

    /// <summary>
    /// 十字架レーザー弾
    /// </summary>
    [System.Serializable]
    public class CrossLazerParam : IBulletCreateData
    {
        public override IBulletMoveData CreateMoveData() => null;
        //public float Width;
        //public float Length;
        public float MoveSpeed;
        public float AngleSpeed;
    }

    /// <summary>
    /// 曲がったあと、まっすぐ進む弾
    /// </summary>
    [System.Serializable]
    public class CurveAndStraight : IBulletCreateData
    {
        public float MoveSpeed;
        public float SecondMoveSpeed;
        public float DisAcceleration;
        public float Span;
        public float CurveAngle;
        public float AngleSpeed;
        public int Length;

        public CurveAndStraight GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new CurveAndStraightMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                DisAcceleration = DisAcceleration,
                CurveAngle = CurveAngle,
                IsStraight = false,
                AngleSpeed = AngleSpeed,
                SecondMoveSpeed = SecondMoveSpeed,
                Damage = Damage,
            };
        }
    }

    /// <summary>
    /// まっすぐ進んだ後、曲がる弾
    /// </summary>
    [System.Serializable]
    public class StraightAndCurve : IBulletCreateData
    {
        public float MoveSpeed;
        public float SecondMoveSpeed;
        public float DisAcceleration;
        public float Span;
        public float CurveAngle;
        public float AngleSpeed;
        public int Length;

        public StraightAndCurve GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new StraightAndCurveMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                DisAcceleration = DisAcceleration,
                CurveAngle = CurveAngle,
                IsStraight = true,
                AngleSpeed = AngleSpeed,
                SecondMoveSpeed = SecondMoveSpeed,
                Damage = Damage,
            };
        }
    }

    /// <summary>
    /// まっすぐ進んだ後、曲がる弾
    /// </summary>
    [System.Serializable]
    public class Curve : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpeed;

        public Curve GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new CurveMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                AngleSpeed = AngleSpeed,
                Damage = Damage,
            };
        }
    }

    /// <summary>
    /// まっすぐ進んだ後、曲がる弾
    /// </summary>
    [System.Serializable]
    public class MultiCurve : IBulletCreateData
    {
        public float MoveSpeed;
        public float AngleSpeed;
        public float Span;
        public float Value;

        public MultiCurve GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new CurveMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                AngleSpeed = AngleSpeed,
                Damage = Damage,
            };
        }
    }

    /// <summary>
    /// 画面端到達で反射する弾
    /// </summary>
    [System.Serializable]
    public class Bounce : IBulletCreateData
    {
        public float MoveSpeed;
        public int BounceLimit;

        public Bounce GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new BounceMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                Damage = Damage,
                BounceLimit = BounceLimit
            };
        }
    }

    /// <summary>
    /// 画面端到達で反射する弾を複数発射する
    /// </summary>
    [System.Serializable]
    public class MultiBounce : IBulletCreateData
    {
        public float MoveSpeed;
        public int BounceLimit;
        public int Value;
        public float Span;
        
        public MultiBounce GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new BounceMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                Damage = Damage,
                BounceLimit = BounceLimit
            };
        }
    }

    /// <summary>
    /// 画面端到達で反射する弾を輪っか状に複数発射する
    /// </summary>
    [System.Serializable]
    public class BounceRing : IBulletCreateData
    {
        public float MoveSpeed;
        public int BounceLimit;
        public float Distance;

        public BounceRing GetData() => this;
        public override IBulletMoveData CreateMoveData()
        {
            return new BounceMove
            {
                MoveDir = Dir,
                MoveSpeed = MoveSpeed,
                Acceleration = Acceleration,
                Damage = Damage,
                BounceLimit = BounceLimit
            };
        }
    }
}
