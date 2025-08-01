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
                { BulletData = new BulletStructs.StaraightShoot { } };

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
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                    }
                };

            // 2
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵:
                return new EnemyDataStructs.NoAction
                { BulletData = new BulletStructs.StaraightShoot { } };

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
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet
                    }
                };

            // 4
            case EnemyEnums.EnemyID.ミサイル敵を撃つ敵:
                return new EnemyDataStructs.SummonEnemy
                {
                    ActionInterval = 5.0f,
                    Target = PlayerManager.Instance.Player,
                    SummonID = EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵,
                    BulletData = new BulletStructs.StaraightShoot { }
                };

            // 5
            case EnemyEnums.EnemyID.輪っか弾を撃つ敵:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 2.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.RingShot
                    {
                        Acceleration = 0.0f,
                        DisAcceleration = 2.0f,
                        ColCategory = ColliderCategory.EnemyBullet,
                        MoveSpeed = 5.5f,
                        Scale = Vector3.one * 1.0f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet
                    }
                };

            // 6
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵:
                return new EnemyDataStructs.NoAction
                { BulletData = new BulletStructs.StaraightShoot { } };

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
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
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
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet
                    }
                };

            // 9
            case EnemyEnums.EnemyID.バリア突進敵:
                return new EnemyDataStructs.SummonEnemy
                {
                    ActionInterval = 9999.0f,
                    Target = PlayerManager.Instance.Player,
                    SummonID = EnemyEnums.EnemyID.渦巻ぐるぐる敵,
                    BulletData = new BulletStructs.StaraightShoot { }
                };

            // 10
            case EnemyEnums.EnemyID.レーザー発射敵:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 1.0f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.LazerParam
                    {
                        MoveSpeed = 0.0f,
                        Acceleration = 0.0f,
                        OpenTime = 0.3f,
                        KeepTime = 3.0f,
                        CloseTime = 0.2f,
                        Width = 3.0f,
                        Length = 20.0f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                    }
                };
        }

        return null;
    }

}
