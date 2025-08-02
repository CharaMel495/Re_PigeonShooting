using UnityEngine;

/// <summary>
/// メインシーンについて管理するクラス
/// </summary>
public class MainSceneManager : SceneManagerBase<MainSceneManager>
{
    public override void Initialize()
    {
        ColliderManager.Instance.Initialize();
        BulletManager.Instance.Initialize();
        PlayerManager.Instance.Initialize();
        EnemyManager.Instance.Initialize();

        _ = CRISoundManager.Instance.Initialize();

        //EnemyManager.Instance.CreateEnemy(0, new Vector3(3.0f, 1.5f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(0, new Vector3(1.5f, -1.5f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(0, new Vector3(4.5f, -1.5f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(1, new Vector3(17.0f, 0.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(2, new Vector3(13.0f, -4.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(3, new Vector3(10.0f, -6.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(4, new Vector3(12.0f, 0.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(5, new Vector3(10.0f, 0.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(6, new Vector3(11.0f, -8.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(7, new Vector3(10.0f, 0.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(8, new Vector3(10.0f, -10.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(2, new Vector3(10.0f, -10.0f, 0.0f));
        //EnemyManager.Instance.CreateEnemy(2, new Vector3(10.0f, -10.0f, 0.0f));
    }

    private void Update()
    {
        if (InputManager.CheckKey(KeyCode.E, InputHandler.Player))
        {
            EnemyManager.Instance.CreateSpiralEnemy(new Vector3(3.0f, 0.0f, 0.0f), 2, false);
            EnemyManager.Instance.CreateSpiralEnemy(new Vector3(3.0f, 0.0f, 0.0f), 2, true);

        }
    }
}
