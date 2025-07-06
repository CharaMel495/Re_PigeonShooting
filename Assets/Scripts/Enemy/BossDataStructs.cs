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
        public Transform transform { get; set; }
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
                    transform.position,
                    SummonEnemyVal,
                    3.0f,
                    UnityEngine.Random.Range(0, 100) % 2 == 0,
                    transform);

            RemainInterval = ActionInterval;

            _currentMode = Mode.ReleaseEnemys;
        }

        private void ReleaseEnemys()
        {
            foreach (var enemy in BarrierEnemyes.ToArray())
            {
                EnemyDataStructs.SpiralMove moveData = (EnemyDataStructs.SpiralMove)enemy.MoveData;
                moveData.SpiralRatio = moveData.SpiralRatio * 0.2f;
                enemy.MoveData = moveData;
            }

            _currentMode = Mode.End;
        }
    }

    public struct BossParam
    {
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public Action<int> OnDamageCallback { get; set; }
    }
}
