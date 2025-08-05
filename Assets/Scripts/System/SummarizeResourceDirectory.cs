using UnityEngine;

/// <summary>
/// 外部ファイルのディレクトリをまとめたクラス
/// </summary>
public class SummarizeResourceDirectory
{
    // TextureDirectory : Addressables

    public const string PLAYERBULLET_TEX = "Assets/Textures/PlayerShot.png";
    public const string SIMPLEENEMY_TEX = "Assets/Textures/EnemyOne.png";
    public const string PLAYER_TEX = "Assets/Textures/Player.png";
    public const string PLAYERLAZER_TEX = "Assets/Textures/PlayerLazer.png";
    public const string SPINNINGENEMY_TEX = "Assets/Textures/CandyBall.png";
    public const string BOSSSHIP_TEX = "Assets/Textures/BossShip.png";
    public const string MISSILE_TEX = "Assets/Textures/IceMissile.png";
    public const string ENEMYBULLET01_TEX = "Assets/Textures/SimpleBullet.png";
    public const string HEALITEM_TEX = "Assets/Textures/HealBattery.png";
    public const string POWITEM_TEX = "Assets/Textures/PoweBattery.png";
    public const string DUSTITEM_TEX = "Assets/Textures/Dust.png";

    // ScriptableObjectDirectory

    public const string ENEMYTABLE_PATH = "/ParameterControll/JsonFiles/EnemyTable.json";
    public const string ENEMYTABLEASSET_PATH = "Assets/ExternalResources/EnemyParamTable.asset";

    ///CRIPath///
    
    public const string CRI_ACFFILE_PATH = "Assets/StreamingAssets/CRIAssets/Re_PegionShooting.acf";
    public const string CRI_ACBFILE_PATH_TEMPLATE = "CRIAssets/";
}
