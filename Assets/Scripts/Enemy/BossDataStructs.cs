using System;
using UnityEngine;

namespace BossDataStructs
{
    public interface IBossAction
    {
        public float ActionInterval { get; }
        public float RemainInterval { get; set; }
        public bool IsFinished();
        public void Act();
    }

    public struct SpreadBarrage : IBossAction
    {
        public int SummonEnemyVal { get; set; }
        public Transform transform { get; set; }
        public float ActionInterval { get; set; }
        public float RemainInterval { get; set; }

        public bool IsFinished()
        {
            return false;
        }

        public void Act()
        {
            if (RemainInterval < 0.1f)
                EnemyManager.Instance.CreateSpiralBarrierEnemy(
                    transform.position,
                    SummonEnemyVal,
                    UnityEngine.Random.Range(0, 100) % 2 == 0,
                    transform);

            RemainInterval = ActionInterval;
        }
    }

    public struct BossParam
    {
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public Action<int> OnDamageCallback { get; set; }
    }
}
