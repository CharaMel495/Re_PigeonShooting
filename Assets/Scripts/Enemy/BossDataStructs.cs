using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossDataStructs
{
    public interface IBossAction
    {
        public float ActionInterval { get; }
        public float RemainInterval { get; set; }
        public void Initialize();
        public bool IsFinished();
        public void Act();
        public void OnDestroyed();
    }

    public class SpreadBarrage : IBossAction
    {
        private enum Mode
        {
            SpawnEnemys,
            ReleaseEnemys,
            End
        }

        public int SummonEnemyVal { get; set; }
        public Transform Transform { get; set; }
        public float ActionInterval { get; set; }
        public float RemainInterval { get; set; }
        public List<Enemy> BarrierEnemyes { get; set; }
        private Mode _currentMode;
        private Dictionary<Mode, Action> _stateMachine;

        public void Initialize()
        {
            _stateMachine = new()
            {
                { Mode.SpawnEnemys, SpawnEnemys },
                { Mode.ReleaseEnemys, ReleaseEnemys }
            };

            _currentMode = Mode.SpawnEnemys;
            RemainInterval = 0.0f;
        }

        public bool IsFinished()
        {
            return _currentMode == Mode.End;
        }

        public void Act()
        {
            if (_currentMode != Mode.End)
                _stateMachine[_currentMode].Invoke();
        }

        private void SpawnEnemys()
        {
            if (RemainInterval < 0.1f)
                BarrierEnemyes = EnemyManager.Instance.CreateSpiralBarrierEnemy(
                    (int)EnemyEnums.EnemyID.渦巻ぐるぐる敵_後方3way,
                    Transform.position,
                    SummonEnemyVal,
                    3.0f,
                    UnityEngine.Random.Range(0, 100) % 2 == 0,
                    Transform);

            RemainInterval = ActionInterval;

            _currentMode = Mode.ReleaseEnemys;
        }

        private void ReleaseEnemys()
        {
            foreach (var enemy in BarrierEnemyes.ToArray())
            {
                EnemyDataStructs.SlavedSpiralMove moveData = (EnemyDataStructs.SlavedSpiralMove)enemy.MoveData;
                enemy.MoveData = new EnemyDataStructs.StrainghtNormalMove
                {
                    MoveDir = moveData.MoveDir,
                    MoveSpeed = 8.0f,
                    Acceleration = -12.0f
                };

                enemy.transform.parent = null;
            }

            

            _currentMode = Mode.End;
        }

        public void OnDestroyed()
        {
            foreach (var enemy in BarrierEnemyes.ToArray())
            {
                EnemyDataStructs.SlavedSpiralMove moveData = (EnemyDataStructs.SlavedSpiralMove)enemy.MoveData;
                enemy.MoveData = new EnemyDataStructs.StrainghtNormalMove
                {
                    MoveDir = moveData.MoveDir,
                    MoveSpeed = 8.0f,
                    Acceleration = -12.0f
                };

                enemy.transform.parent = null;
            }
        }
    }

    public class ShootMissiles : IBossAction
    {
        public float ActionInterval { get; set; }
        public float RemainInterval { get; set; }
        public Transform Transform { get; set; }
        private int ShotCount { get; set; }
        public int ShotValue { get; set; }

        public void Initialize()
        {
            ShotCount = 0;
        }

        public bool IsFinished()
        {
            return ShotCount >= ShotValue;
        }

        public void Act()
        {
            EnemyManager.Instance.CreateEnemy((int)EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵_弾あり,
                Transform.position, GetRandomAngle());

            RemainInterval = ActionInterval;

            ++ShotCount;
        }

        private Vector3 GetRandomAngle()
        {
            float randomAngle = UnityEngine.Random.Range(-90f, 90f);
            Vector3 dir = Quaternion.Euler(0, 0, randomAngle) * Vector3.right;

            return dir;
        }

        public void OnDestroyed()
        {

        }
    }

    public class DiscShot : IBossAction
    {
        public float ActionInterval { get; set; }
        public float RemainInterval { get; set; }
        public Transform Transform { get; set; }
        public BulletStructs.RingShot LittleRing { get; set; }
        public BulletStructs.RingShot BigRing { get; set; }
        private int ShotCount { get; set; }
        public int ShotValue { get; set; }

        public void Initialize()
        {
            ShotCount = 0;
        }

        public bool IsFinished()
        {
            return ShotCount >= ShotValue;
        }

        public void Act()
        {
            var bigRing = BigRing;
            bigRing.Origin = Transform.position;
            var littleRing = LittleRing;
            littleRing.Origin = Transform.position;

            BulletManager.Instance.Shooter.Shoot(bigRing);
            BulletManager.Instance.Shooter.Shoot(littleRing);

            RemainInterval = ActionInterval;

            ++ShotCount;
        }

        public void OnDestroyed()
        {

        }
    }

    public struct BossParam
    {
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public Action<int> OnDamageCallback { get; set; }
    }
}
