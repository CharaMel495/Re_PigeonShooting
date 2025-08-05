using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SelfMade
{
    public enum SliderDir
    {
        RightToLeft,
        LeftToRight,
        BottomToTop,
        TopToBottom
    }

    /// <summary>
    /// スライダーUI管理クラス
    /// </summary>
    public class Slider : MonoBehaviour
    {
        [SerializeField, Range(0, 1)]
        [Tooltip("スライダーの進行割合")]
        private float _value;

        [SerializeField]
        [Tooltip("スライダーにかけているマスク")]
        private RectMask2D _sliderMask;

        [SerializeField]
        [Tooltip("スライダーにかけているマスク")]
        private Image _sliderImage;

        [SerializeField]
        private SliderDir _sliderDir;

        /// <summary>
        /// スライダーの色変更をする関数
        /// </summary>
        /// <param name="color">変更後の色</param>
        public void ChangeSliderColor(Color color)
            => _sliderImage.color = color;

        /// <summary>
        /// 直接valueを更新する関数
        /// </summary>
        public void UpdateValue(float value)
        {
            _value = value;

            if (_value > 1)
                _value = 1;
            else if (_value < 0)
                _value = 0;

            //UIの更新
            ApplyValue();
        }

        /// <summary>
        /// 現在値と最大値からvalueを更新する関数
        /// </summary>
        /// <param name="currentVal">現在の量</param>
        /// <param name="maxVal">最大値</param>
        public void UpdateValue(float currentVal, float maxVal)
        {
            _value = Mathf.Clamp01(currentVal / maxVal);

            if (_value > 1)
                _value = 1;
            else if (_value < 0)
                _value = 0;

            //UIの更新
            ApplyValue();
        }

        /// <summary>
        /// valueの値をスライダーに反映させる関数
        /// </summary>
        private void ApplyValue()
        {
            //パディングを取得
            var padding = _sliderMask.padding;

            SetPadding(ref padding);

            //更新を適用
            _sliderMask.padding = padding;
        }

        public void SetPadding(ref Vector4 padding)
        {
            //横方向のスケールを取得
            float scaleX = this.gameObject.GetComponent<RectTransform>().localScale.x;
            //padding.xの最大値を計算
            float maxValue = scaleX * 100;

            padding = Vector4.zero;

            switch (_sliderDir)
            {
                case SliderDir.RightToLeft:
                    //限界値を超えないようにパディングを更新
                    padding.x = (maxValue - (_value * scaleX) * 100);
                    if (padding.x > maxValue)
                        padding.x = maxValue;
                    else if (padding.x < 0)
                        padding.x = 0;
                    break;

                case SliderDir.LeftToRight:
                    //限界値を超えないようにパディングを更新
                    padding.z = (maxValue - (_value * scaleX) * 100);
                    if (padding.z > maxValue)
                        padding.z = maxValue;
                    else if (padding.z < 0)
                        padding.z = 0;
                    break;

                case SliderDir.BottomToTop:
                    //限界値を超えないようにパディングを更新
                    padding.w = (maxValue - (_value * scaleX) * 100);
                    if (padding.w > maxValue)
                        padding.w = maxValue;
                    else if (padding.w < 0)
                        padding.w = 0;
                    break;

                case SliderDir.TopToBottom:
                    //限界値を超えないようにパディングを更新
                    padding.y = (maxValue - (_value * scaleX) * 100);
                    if (padding.y > maxValue)
                        padding.y = maxValue;
                    else if (padding.y < 0)
                        padding.y = 0;
                    break;
            }
        }

#if UNITY_EDITOR

        /// <summary>
        /// エディタの拡張部分
        /// </summary>
        [CustomEditor(typeof(SelfMade.Slider))]
        public class SliderCustomEditor : Editor
        {
            //インスペクターに描画するとき
            public override void OnInspectorGUI()
            {
                //変更の監視を開始
                EditorGUI.BeginChangeCheck();

                //インスペクターを描画
                base.OnInspectorGUI();

                //監視を停止し、もし変更が加わっていたら
                if (EditorGUI.EndChangeCheck())
                {
                    if (target is SelfMade.Slider slider)
                    {
                        //スライダーの値を更新
                        slider.ApplyValue();
                    }
                }
            }
        }

#endif
    }
}