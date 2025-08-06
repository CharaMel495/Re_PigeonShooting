using UnityEngine;

/// <summary>
/// シングルトン化したMonoBehaviour基底クラス（FindObjectOfType不使用）
/// </summary>
/// <typeparam name="T">クラステンプレート</typeparam>
public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static T Instance => _instance;

    /// <summary>
    /// Awake時にシングルトン登録
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(gameObject); // シーンを跨いで生存させる
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // 他のインスタンスを破棄
        }
    }

    /// <summary>
    /// アプリ終了時にインスタンスをリセット
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}
