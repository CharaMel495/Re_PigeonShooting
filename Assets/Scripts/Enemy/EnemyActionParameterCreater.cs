using UnityEngine;

public class EnemyActionParameterCreater
{
    public EnemyDataStructs.IEnemyActionData GetEnemyActionData(EnemyEnums.EnemyID actionType)
    {
        switch (actionType)
        {
            // 0
            case EnemyEnums.EnemyID.キホンの雑魚敵:
                return new EnemyDataStructs.NoAction
                {};

            // 1
            case EnemyEnums.EnemyID.キホンの弾を撃つ敵:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.StaraightShoot
                    {
                        MoveSpeed = 4.0f,
                        Acceleration = 0.0f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                    }
                };

            // 2
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵:
                return new EnemyDataStructs.NoAction
                {};

            // 3
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵_弾あり:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.ThreeWayShoot
                    {
                        MoveSpeed = 3.0f,
                        Acceleration = 0.0f,
                        AngleSpan = 30.0f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet
                    }
                };

            // 4
            case EnemyEnums.EnemyID.ミサイル敵を撃つ敵:
                return new EnemyDataStructs.SummonEnemy
                {

                };

            // 5
            case EnemyEnums.EnemyID.輪っか弾を撃つ敵:
                return new EnemyDataStructs.SimpleAction
                {

                };

            // 6
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵:
                return new EnemyDataStructs.NoAction
                {};

            // 7
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵_自機狙い単発弾:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.StaraightShoot
                    {
                        MoveSpeed = 4.0f,
                        Acceleration = 0.0f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                    }
                };

            // 8
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵_後方3way:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.ThreeWayShoot
                    {
                        MoveSpeed = 3.0f,
                        Acceleration = 0.0f,
                        AngleSpan = 30.0f,
                        SpriteType = SpriteData.SpriteType.PlayerBullet,
                        ColCategory = ColliderCategory.EnemyBullet
                    }
                };

            // 9
            case EnemyEnums.EnemyID.バリア突進敵:
                return new EnemyDataStructs.SummonEnemy
                {

                };

            // 10
            case EnemyEnums.EnemyID.レーザー発射敵:
                return new EnemyDataStructs.SimpleAction
                {

                };
        }

        return null;
    }

}
