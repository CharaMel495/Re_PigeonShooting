using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

/// <summary>
/// 文字列UIを使いやすくするためにTextをラッピングしたクラス
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextWrapper : MonoBehaviour
{
    /// <summary>
    /// ラッピングされたテキスト
    /// </summary>
    private TextMeshProUGUI _text;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        if (_text != null)
            return;

        _text = this.GetComponent<TextMeshProUGUI>();
        var fontAssset = Addressables.LoadAssetAsync<TMP_FontAsset>(SummarizeResourceDirectory.FONT).WaitForCompletion();
        _text.font = fontAssset;
    }


    /// <summary>
    /// 現在の文字列を取得
    /// </summary>
    /// <returns>現在の文字列</returns>
    public string GetText()
        => _text.text;

    /// <summary>
    /// 文字列に文字列を追加する関数
    /// </summary>
    /// <param name="addText">追加したい文字列</param>
    public void AddText(string addText)
        => _text.text += addText;

    /// <summary>
    /// 文字列に文字を追加する関数
    /// </summary>
    /// <param name="addText">追加したい文字列</param>
    public void AddText(char addText)
        => _text.text += addText;

    /// <summary>
    /// 指定の文字列に置き換える関数
    /// </summary>
    /// <param name="setText">置き換え先の文字列</param>
    public void SetText(string setText)
        => _text.text = setText;

    /// <summary>
    /// 文字列を消去する関数
    /// </summary>
    public void ClearText()
        => _text.text = "";

    /// <summary>
    /// フォントサイズを更新する関数
    /// </summary>
    /// <param name="size">新しいフォントサイズ</param>
    public void SetFontSize(float size)
        => _text.fontSize = size;

    /// <summary>
    /// 現在の色を取得する関数
    /// </summary>
    public Color GetCurrentColor()
        => _text.color;

    /// <summary>
    /// 現在の色を更新する関数
    /// </summary>
    /// <param name="color"></param>
    public void SetTextColor(Color color)
        => _text.color = color;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public float GetTextAlpha()
        => _text.color.a;

    /// <summary>
    /// 文字UIの透明度を取得する関数
    /// </summary>
    public void SetTextAlpha(float value)
        => _text.color = new (_text.color.r, _text.color.g, _text.color.b, value);
}
