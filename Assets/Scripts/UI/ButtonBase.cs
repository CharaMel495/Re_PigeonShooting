using DG.Tweening;
using System;
using UnityEngine;

[System.Serializable]
public class ButtonMovingRuler
{
    public int UpButtonNum;
    public int RightButtonNum;
    public int LeftButtonNum;
    public int DownButtonNum;
}

public abstract class ButtonBase : MonoBehaviour
{
    protected Action _onPressedFunc;

    public ButtonMovingRuler MoveRuler;

    public abstract bool IsMoving { get; }

    public abstract void Initialize(Action func = null);

    /// <summary>
    /// このボタンが選択された際の処理
    /// </summary>
    public void Selected()
        => _onPressedFunc?.Invoke();

    public abstract bool EnActive();

    public abstract bool DisActive();
}
