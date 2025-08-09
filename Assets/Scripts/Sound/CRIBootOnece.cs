using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class CriBootOnce : MonoBehaviour
{
    private static CriBootOnce _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            // 2体目以降は即破棄（シーン戻り・再入場での重複対策）
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);
        // ここで特に何もしない。各コンポーネント(CriWareLibraryInitializer 等)がAwakeで初期化してくれる
    }
}
