using UnityEngine;

/// <summary>
/// 自身の座標を公開するクラスが継承するインターフェース
/// </summary>
public interface ITargetProvider
{
    public Vector3 GetPostion();
}
