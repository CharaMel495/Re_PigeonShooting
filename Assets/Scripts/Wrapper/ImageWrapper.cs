using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画像UIを使いやすくするためにImageをラッピングしたクラス
/// </summary>
[RequireComponent(typeof(Image))]
public class ImageWrapper : MonoBehaviour
{
    /// <summary>
    /// ラップ対象
    /// </summary>
    private Image _image;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        if (_image != null)
            return;

        _image = this.GetComponent<Image>();
    }

    /// <summary>
    /// 画像をセットする関数
    /// </summary>
    /// <param name="sprite">セットしたい画像</param>
    public void SetSprite(Sprite sprite)
        => _image.sprite = sprite;

    /// <summary>
    /// 現在の色を取得する関数
    /// </summary>
    public Color GetCurrentColor()
        => _image.color;

    /// <summary>
    /// 現在の色を更新する関数
    /// </summary>
    /// <param name="color"></param>
    public void SetImageColor(Color color)
        => _image.color = color;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public float GetImageAlpha()
        => _image.color.a;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public void SetImageAlpha(float value)
        => _image.color = new(_image.color.r, _image.color.g, _image.color.b, value);

    public void SetFillAmount(float value)
    {
        if (_image.type != Image.Type.Filled)
            return;

        _image.fillAmount = Mathf.Clamp01(value);
    }
}
