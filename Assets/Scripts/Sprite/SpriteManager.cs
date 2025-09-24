using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SpriteData
{
    public enum SpriteType
    {
        // プレイヤー系　0番台
        Player = 0,
        PlayerBullet,
        PlayerLazer,
        // 敵系　100番台
        NormalEnemy = 100,
        Missile,
        SpinningEnemy,
        BossShip,
        EnemyBullet,
        // アイテム系　200番台
        HealItem = 200,
        PowItem,
        Dust,
        EXP
        // システム(UI)系 300番台
    }
}

/// <summary>
/// 画像管理クラス
/// </summary>
public class SpriteManager
{
    /// <summary>
    /// 指定の画像を読み込む関数
    /// </summary>
    /// <param name="id">スプライトの指定enum</param>
    /// <returns>指定されたスプライト</returns>
    public static Sprite GetSprite(SpriteData.SpriteType id)
    {
        // 画像を読み込み、返す
        return LoadSprite(GetSpritePath());

        // enumからパスに変換する関数
        string GetSpritePath()
        {
            return id switch
            {
                SpriteData.SpriteType.PlayerBullet => SummarizeResourceDirectory.PLAYERBULLET_TEX,
                SpriteData.SpriteType.NormalEnemy => SummarizeResourceDirectory.SIMPLEENEMY_TEX,
                SpriteData.SpriteType.Player => SummarizeResourceDirectory.PLAYER_TEX,
                SpriteData.SpriteType.PlayerLazer => SummarizeResourceDirectory.PLAYERLAZER_TEX,
                SpriteData.SpriteType.SpinningEnemy => SummarizeResourceDirectory.SPINNINGENEMY_TEX,
                SpriteData.SpriteType.BossShip => SummarizeResourceDirectory.BOSSSHIP_TEX,
                SpriteData.SpriteType.Missile => SummarizeResourceDirectory.MISSILE_TEX,
                SpriteData.SpriteType.EnemyBullet => SummarizeResourceDirectory.ENEMYBULLET01_TEX,
                SpriteData.SpriteType.HealItem => SummarizeResourceDirectory.HEALITEM_TEX,
                SpriteData.SpriteType.PowItem => SummarizeResourceDirectory.POWITEM_TEX,
                SpriteData.SpriteType.Dust => SummarizeResourceDirectory.DUSTITEM_TEX,
                SpriteData.SpriteType.EXP => SummarizeResourceDirectory.EXPITEM_TEX,
                _ => null
            };
        }
    }

    /// <summary>
    /// Addressablesアセット化した画像をSpriteクラスに変換する関数
    /// </summary>
    /// <param name="path">Sprite化したい画像のディレクトリ</param>
    /// <returns>変換した画像</returns>
    private static Sprite LoadSprite(string path)
    {
        try
        {
            // アセットをTexture2Dとしてロード
            Texture2D texture2D = Addressables.LoadAssetAsync<Texture2D>(path).WaitForCompletion();
            // Texture2DからSpriteクラスを生成
            var sprite = Sprite.Create(
                texture2D,                                           //使うテクスチャ
                new Rect(0f, 0f, texture2D.width, texture2D.height), //テクスチャ矩形
                new Vector2(0.5f, 0.5f),                             //テクスチャ原点
                (texture2D.width + texture2D.height) * 0.5f);        //画像の粗さ
            // 生成した画像を返却
            return sprite;
        }
        catch (Exception e)
        {
            // ログにエラーを書いて終了
            Debug.LogError(e.Message);
            return null;
        }
    }
}
