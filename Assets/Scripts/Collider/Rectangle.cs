using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SelfMade
{
    /// <summary>
    /// 矩形クラス
    /// </summary>
    public partial class Rectangle : ICollider
    {
        public int ID { get; set; }

        /// <summary>
        /// アタッチされてるオブジェクトの
        /// トランスフォーム
        /// コンストラクタで受け取る
        /// </summary>
        private Transform _transform;

        /// <summary>
        /// ４頂点の座標
        /// [0] = 左上
        /// [1] = 右上
        /// [2] = 右下
        /// [3] = 左下
        /// </summary>
        public Vector3[] Verts
        { get; private set; }

        /// <summary>
        /// 外接円
        /// </summary>
        public Circle _outerCircle;

        /// <summary>
        /// 横方向の大きさ
        /// 除算の回数を減らすために変数を用意
        /// </summary>
        public float LocalScale_X
        { get; private set; }

        /// <summary>
        /// 縦方向の大きさ
        /// 除算の回数を減らすために変数を用意
        /// </summary>
        public float LocalScale_Y
        { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="transform">このインスタンスを持つオブジェクトのトランスフォーム</param>
        public Rectangle(Transform transform)
        {
            _transform = transform;
            UpdateScale();
            UpdateVerts();
            CreateOuterCircle();
        }

        /// <summary>
        /// 最新の頂点の座標を取得する関数
        /// 左上から時計周りになるように取得
        /// </summary>
        public void UpdateVerts()
        {
            UpdateScale();

            //各頂点に向かうベクトルを取得
            var vectors = GetDirectionVector();

            //初期化
            Verts = new Vector3[4];

            //座標を取得
            Verts[0] = vectors[0] + _transform.position;
            Verts[1] = vectors[1] + _transform.position;
            Verts[2] = vectors[2] + _transform.position;
            Verts[3] = vectors[3] + _transform.position;
        }

        public void CreateOuterCircle()
        {
            UpdateVerts();

            float vectorNorm = (_transform.localScale.x + _transform.localScale.y) * 0.5f;

            _outerCircle = new(_transform);
        }

        /// <summary>
        /// 最新の大きさを取得する関数
        /// </summary>
        public void UpdateScale()
        {
            LocalScale_X = _transform.localScale.x / 2;
            LocalScale_Y = _transform.localScale.y / 2;
        }

        /// <summary>
        /// 4つの頂点に向かう正規化ベクトルを取得する関数
        /// 左上から時計周りになるように取得
        /// </summary>
        private Vector3[] GetDirectionVector()
        {
            //各頂点に向かうベクトルを取得
            Vector2 upLeft = -_transform.right * LocalScale_X + _transform.up * LocalScale_Y;
            Vector2 upRight = _transform.right * LocalScale_X + _transform.up * LocalScale_Y;
            Vector2 downRight = _transform.right * LocalScale_X + -_transform.up * LocalScale_Y;
            Vector2 downLeft = -_transform.right * LocalScale_X + -_transform.up * LocalScale_Y;

            //一旦入れ物に格納(順番を整えるのも兼ねて)
            Vector3[] retruner = { upLeft, upRight, downRight, downLeft };

            //取得した配列を返却
            return retruner;
        }

        /// <summary>
        /// 自身と渡された矩形とで衝突判定を行う関数
        /// </summary>
        /// <param name="otherRect">自身と判定するもう１つの矩形</param>
        /// <returns>衝突でtrue</returns>
        public bool HitJudge(Rectangle otherRect)
        {
            UpdateVerts();
            otherRect.UpdateVerts();

            //頂点のみ使うのでここで取得
            Vector3[] vertsA = this.Verts;
            Vector3[] vertsB = otherRect.Verts;

            //分離軸の候補となる４直線を求める
            //※矩形１つにつき２つの辺方向をもつので４つ候補が挙がる
            Vector2[] sideDirs =
            {
                (vertsA[0] - vertsA[1]).normalized, (vertsA[1] - vertsA[2]).normalized,
                (vertsB[0] - vertsB[1]).normalized, (vertsB[1] - vertsB[2]).normalized
            };

            //分離軸の候補全てから分離軸になりうるかを判定していく
            foreach (var u in sideDirs)
            {
                //矩形の頂点の内１点を直線に射影
                float minA = Vector2.Dot(vertsA[0], u);
                float maxA = minA;
                float minB = Vector2.Dot(vertsB[0], u);
                float maxB = minB;

                //内積を用いて残りの頂点も直線に射影していき
                //各矩形を射影した際の射影区間を表す最大値と最小値を求める
                for (int i = 1; i < 4; i++)
                {
                    float projA = Vector2.Dot(vertsA[i], u);
                    if (projA < minA)
                        minA = projA;
                    else if (projA > maxA)
                        maxA = projA;

                    float projB = Vector2.Dot(vertsB[i], u);
                    if (projB < minB)
                        minB = projB;
                    else if (projB > maxB)
                        maxB = projB;
                }

                //２つの射影区間の間に隙間があれば分離軸が存在するので
                //逆説的に２矩形が衝突してないことが証明される
                if (maxB < minA || maxA < minB)
                    return false;
            }

            //分離軸が存在してないので
            //逆説的に２矩形は衝突していることが証明される
            return true;
        }

        /// <summary>
        /// 円との当たり判定
        /// </summary>
        /// <param name="circle">対象の円</param>
        /// <returns>当たっているか</returns>
        public bool HitJudge(Circle circle)
        {
            UpdateVerts();

            //もし外接円に触れてなければすぐ終了
            if (!_outerCircle.HitJudge(circle))
                return false;

            //頂点のみ使うのでここで取得
            Vector3[] vertsA = this.Verts;

            //分離軸の候補となる４直線を求める
            //※矩形１つにつき２つの辺方向をもつので４つ候補が挙がる
            Vector2[] sideDirs =
            {
                (vertsA[0] - vertsA[1]).normalized, (vertsA[1] - vertsA[2]).normalized
            };

            //分離軸の候補全てから分離軸になりうるかを判定していく
            foreach (var u in sideDirs)
            {
                //矩形の頂点の内１点を直線に射影
                float minA = Vector2.Dot(vertsA[0], u);
                float maxA = minA;

                //内積を用いて残りの頂点も直線に射影していき
                //各矩形を射影した際の射影区間を表す最大値と最小値を求める
                for (int i = 1; i < 4; i++)
                {
                    float projA = Vector2.Dot(vertsA[i], u);
                    if (projA < minA)
                        minA = projA;
                    else if (projA > maxA)
                        maxA = projA;
                }

                float projCirclePos = Vector2.Dot(circle.Position, u);

                //射影した円が矩形の射影区間に入ってなければ当たっていない
                if (projCirclePos + circle.Radius < minA || maxA < projCirclePos - circle.Radius)
                    return false;
            }

            //射影した円が矩形の射影区間に入っていたので当たり
            return true;
        }
    }
}