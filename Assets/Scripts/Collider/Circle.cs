using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SelfMade
{
    public partial class Circle : ICollider
    {
        public int ID { get; set; }

        private Transform _transform;

        /// <summary>
        /// 中心
        /// </summary>
        public Vector3 Position
        { get => _transform.position; }

        /// <summary>
        /// 半径
        /// </summary>
        public float Radius
        { get => _transform.localScale.x * 0.5f; }

        /// <summary>
        /// 半径の2乗
        /// </summary>
        public float RadiusPowered
            => Radius * Radius;

        public IColliderbleObject Owner { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos">座標</param>
        /// <param name="rad">半径</param>
        public Circle(Transform transform)
        {
            _transform = transform;
        }

        /// <summary>
        /// 円同士の当たり判定を取る関数
        /// </summary>
        /// <param name="other">他の円</param>
        public bool HitJudge(Circle other)
        {
            float dx = Position.x - other.Position.x;
            float dy = Position.y - other.Position.y;
            float positionDistance = dx * dx + dy * dy;

            float radSum = Radius + other.Radius;
            float radSumSquared = radSum * radSum;

            return positionDistance <= radSumSquared;

        }

        /// <summary>
        /// その点が円に含まれているかを調べる関数
        /// </summary>
        /// <param name="point">調べたい座標</param>
        public bool HitJudge(Vector3 point)
        {
            float distance =
                Mathf.Pow(Position.x - point.x, 2) +
                Mathf.Pow(Position.y - point.y, 2);

            return distance <= RadiusPowered;
        }
    }
}