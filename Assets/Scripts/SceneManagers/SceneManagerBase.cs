using System;
using UnityEngine;

/// <summary>
/// 各シーン管理クラスに継承させるクラス
/// </summary>
/// <typeparam name="TScene">継承先シーンマネージャー</typeparam>
public abstract class SceneManagerBase<TScene> : SingletonMonoBehaviour<SceneManagerBase<TScene>>
{
    /// <summary>
    /// シーンマネージャーとしてのインスタンス
    /// </summary>
    private static TScene _sceneInstance;

    /// <summary>
    /// 継承したシーンマネージャーとしてインスタンスを取得するメソッド
    /// </summary>
    /// <returns></returns>
    public TScene GetSceneManagerInstance()
    {
        // If the instance is null
        if (_sceneInstance == null)
        {
            // Get the type of the scene
            Type t = typeof(TScene);

            // Find the first object of the specified type and cast it to TScene
            _sceneInstance = (TScene)(object)FindFirstObject(t);
        }

        // Return the instance
        return _sceneInstance;
    }

    /// <summary>
    /// インスタンスが未登録のときにインスタンスを探し出すメソッド
    /// </summary>
    /// <param name="type">型</param>
    /// <returns>探し出したインスタンス</returns>
    private static UnityEngine.Object FindFirstObject(Type type)
    {
        // Use Unity's new API to find the first object of the specified type
        return FindFirstObjectByType(type);
    }

    public abstract void Initialize();
}
