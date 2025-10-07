using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDefaultStatus", menuName = "Scriptable Objects/PlayerDefaultStatus")]
public class PlayerDefaultStatus : ScriptableObject
{
    // 一回の射撃で発射する弾の数
    public int ShotValue;
    // 弾一発の攻撃力
    public int ShotPower;
    // 弾の発射間隔
    //public float MinShotRate = 0.04f;
    //public float MaxShotRate = 0.4f;
    public float ShotRate = 0.6f;
    public float LessShotProbabirity = 100;
    // 被弾後無敵時間
    public float InvincibleTime = 0.1f;
    // ダッシュ継続時間
    public float DashTime = 0.1f;
    // 成長する順番
    public PlayerGrowStatus.GrowStatus[] GrowOrder;
    // ほこりの溜まり具合に応じた行動
    public PlayerBullet.DustAction[] DustActions;
}
