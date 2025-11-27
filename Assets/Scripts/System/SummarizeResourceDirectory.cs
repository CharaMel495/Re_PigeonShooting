using UnityEngine;

/// <summary>
/// 外部ファイルのディレクトリをまとめたクラス
/// </summary>
public class SummarizeResourceDirectory
{
    // TextureDirectory : Addressables

    public const string PLAYERBULLET_TEX = "Assets/Textures/PlayerShot.png";
    public const string SIMPLEENEMY_TEX = "Assets/Textures/Candy_pixel.png";
    public const string PLAYER_TEX = "Assets/Textures/Player.png";
    public const string PLAYERLAZER_TEX = "Assets/Textures/Bullet/LazerPixel.png";
    public const string SPINNINGENEMY_TEX = "Assets/Textures/CandyBall.png";
    public const string BOSS01_TEX = "Assets/Textures/Coolie_pixel.png";
    public const string MISSILE_TEX = "Assets/Textures/IceMissile.png";
    public const string ENEMYBULLET01_TEX = "Assets/Textures/Bullet/Enemy_Bullet2.png";
    public const string LAZERBULLET01_TEX = "Assets/Textures/Bullet/LazerBullet.png";
    public const string HEALITEM_TEX = "Assets/Textures/HealBattery.png";
    public const string POWITEM_TEX = "Assets/Textures/PoweBattery.png";
    public const string DUSTITEM_TEX = "Assets/Textures/Dust.png";
    public const string EXPITEM_TEX = "Assets/Textures/Item/EXP_Green.png";
    public const string TUTORIALTEXTS = "Assets/ExternalResources/TutorialTexts.asset";
    public const string BULLETPOWERUPICON_TEX = "Assets/Textures/UI/GrowUI/Icon_BulletPowerUp.png";
    public const string BULLETRATEUPICON_TEX = "Assets/Textures/UI/GrowUI/Icon_BulletRateUp.png";
    public const string BULLETUPICON_TEX = "Assets/Textures/UI/GrowUI/Icon_BulletUp.png";
    public const string BULLETDUSTPROOFUPICON_TEX = "Assets/Textures/UI/GrowUI/Icon_DustProof.png";
    public const string RINGBOMB_TEX = "Assets/Textures/Bullet/UI_G_Ring.png";

    // UsingFont
    public const string FONT = "Assets/Font/PixelMplus12-Regular SDF.asset";

    //public const string FONT = "Assets/Font/KH-Dot-Dougenzaka-16 SDF.asset";

    // ScriptableObjectDirectory

    public const string ENEMYTABLE_PATH = "/ParameterControll/JsonFiles/EnemyTable.json";
    public const string ENEMYTABLEASSET_PATH = "Assets/ExternalResources/EnemyParamTable.asset";
    public const string WAVEEVENTTABLE_PATH = "/ParameterControll/JsonFiles/WaveEventTable.json";
    public const string WAVEEVENTTABLEASSET_PATH = "Assets/ExternalResources/WaveEventTable.asset";
    public const string WAVETIMETABLE_PATH = "/ParameterControll/JsonFiles/WaveTimeTable.json";
    public const string WAVETIMETABLEASSET_PATH = "Assets/ExternalResources/WaveTimeTable.asset";

    ///CRIPath///

    public const string CRI_ACFFILE_PATH = "Assets/StreamingAssets/CRIAssets/Re_PegionShooting.acf";
    public const string CRI_ACBFILE_PATH_TEMPLATE = "CRIAssets/";
}
