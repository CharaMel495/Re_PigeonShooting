using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 画像クラスを使いやすくするためにSpriteRendererをラッピングしたクラス
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererWrapper : MonoBehaviour
{
    /// <summary>
    /// ラップ対象
    /// </summary>
    private SpriteRenderer _renderer;

    /// <summary>
    /// 左右反転フラグ
    /// </summary>
    public bool FlipX
        => _renderer.flipX;

    /// <summary>
    /// 上下反転フラグ
    /// </summary>
    public bool FlipY
        => _renderer.flipY;

    /// <summary>
    /// 現在のスプライト
    /// </summary>
    public Sprite CurrentSprite
        => _renderer.sprite;

    /// <summary>
    /// カメラに映っているか
    /// </summary>
    public bool IsInCamera
        => _renderer.isVisible;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        if (_renderer != null)
            return;


        _renderer = this.GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 画像をセットする関数
    /// </summary>
    /// <param name="sprite">セットしたい画像</param>
    public void SetSprite(Sprite sprite)
        => _renderer.sprite = sprite;

    /// <summary>
    /// 画像を左右反転させる関数
    /// </summary>
    public void ReverseX()
        => _renderer.flipX = !_renderer.flipX;

    /// <summary>
    /// 画像の左右反転をセットする関数
    /// </summary>
    /// <param name="flip">反転させるか</param>
    public void SetFlipX(bool flip)
        => _renderer.flipX = flip;

    /// <summary>
    /// 画像を上下反転させる関数
    /// </summary>
    public void ReverseY()
        => _renderer.flipY = !_renderer.flipY;

    /// <summary>
    /// 画像の上下反転をセットする関数
    /// </summary>
    /// <param name="flip">反転させるか</param>
    public void SetFlipY(bool flip)
        => _renderer.flipY = flip;

    /// <summary>
    /// 現在の色を取得する関数
    /// </summary>
    public Color GetCurrentColor()
        => _renderer.color;

    /// <summary>
    /// 現在の色を更新する関数
    /// </summary>
    /// <param name="color"></param>
    public void SetSpriteColor(Color color)
        => _renderer.color = color;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public float GetSpriteAlpha()
        => _renderer.color.a;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public void SetSpriteAlpha(float value)
        => _renderer.color = new(_renderer.color.r, _renderer.color.g, _renderer.color.b, value);

    public void SetEnabled(bool value)
        => _renderer.enabled = value;
}
