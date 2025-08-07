using UnityEngine;

public class TutorialStageManager : SingletonMonoBehaviour<TutorialStageManager>
{
    [SerializeField]
    private Rect _area;
    public Rect PlayArea
        => _area;

    public void Initialize()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public Vector3 GetRandomPositionInArea(float minDistanceFromPlayer = 0)
    {
        ITargetProvider player = TutorialPlayerManager.Instance.Player;
        Vector3 spawnPos;
        int safetyLoop = 0;

        do
        {
            // Rect内のランダム座標を取得
            float x = Random.Range(PlayArea.xMin, PlayArea.xMax);
            float y = Random.Range(PlayArea.yMin, PlayArea.yMax);
            spawnPos = new Vector3(x, y, 0);

            safetyLoop++;
            if (safetyLoop > 100) break; // 無限ループ対策
        } while (Vector3.Distance(spawnPos, player.GetPostion()) * 0.01 < minDistanceFromPlayer);

        return spawnPos;
    }

}
