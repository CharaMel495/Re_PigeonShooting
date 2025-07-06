using UnityEngine;

/// <summary>
/// 外部ファイルのディレクトリをまとめたクラス
/// </summary>
public class SummarizeResourceDirectory
{
    // TextureDirectory : Addressables

    public const string PLAYERBULLET_TEX = "Assets/Textures/SimpleBullet.png";
    public const string SIMPLEENEMY_TEX = "Assets/Textures/EnemyOne.png";
    public const string PLAYER_TEX = "Assets/Textures/Player.png";
    public const string PLAYERLAZER_TEX = "Assets/Textures/PlayerLazer.png";
    public const string SPINNINGENEMY_TEX = "Assets/Textures/CandyBall.png";
    public const string BOSSSHIP_TEX = "Assets/Textures/BossShip.png";
    public const string MISSILE_TEX = "Assets/Textures/IceMissile.png";

    // ScriptableObjectDirectory

    public const string ENEMYTABLE_PATH = "/ParameterControll/JsonFiles/EnemyTable.json";
    public const string ENEMYTABLEASSET_PATH = "Assets/ExternalResources/EnemyParamTable.asset";
}
