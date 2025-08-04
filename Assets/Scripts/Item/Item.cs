using UnityEngine;

public enum ItemType
{
    Battery_Green,
    Battery_Red,
    Garbage
}

/// <summary>
/// 敵が落とすアイテム
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// 吸引されてる対象
    /// 吸引された時に、寄れるように
    /// </summary>
    private ITargetProvider _vacuumedTarget;

    /// <summary>
    /// 移動方向
    /// </summary>
    private Vector3 _dir;

    public void Initialize()
    {

    }
}
