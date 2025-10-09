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
                { BulletData = new BulletStructs.NoBullet { } };

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
                        Damage = 1
                    }
                };

            // 2
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵:
                return new EnemyDataStructs.NoAction
                { BulletData = new BulletStructs.NoBullet { } };

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
                        ColCategory = ColliderCategory.EnemyBullet,
                        Damage = 1
                    }
                };

            // 4
            case EnemyEnums.EnemyID.ミサイル敵を撃つ敵:
                return new EnemyDataStructs.SummonEnemy
                {
                    ActionInterval = 5.0f,
                    Target = PlayerManager.Instance.Player,
                    SummonID = EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵,
                    BulletData = new BulletStructs.NoBullet { }
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
                        DisAcceleration = 4.0f,
                        ColCategory = ColliderCategory.EnemyBullet,
                        MoveSpeed = 5.5f,
                        SecondMoveSpeed = 7.0f,
                        Scale = Vector3.one * 1.0f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        Damage = 1
                    }
                };

            // 6
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵:
                return new EnemyDataStructs.NoAction
                { BulletData = new BulletStructs.NoBullet { } };

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
                        Damage = 1
                    }
                };

            // 8
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵_後方3way:
                return new EnemyDataStructs.SimpleAction
                {
                    ActionInterval = 0.5f,
                    Target = PlayerManager.Instance.Player,
                    BulletData = new BulletStructs.ThreeWayShoot
                    {
                        MoveSpeed = 10.0f,
                        Acceleration = 0.0f,
                        AngleSpan = 30.0f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Damage = 1
                    }
                };

            // 9
            case EnemyEnums.EnemyID.バリア突進敵:
                return new EnemyDataStructs.SummonEnemy
                {
                    ActionInterval = 9999.0f,
                    Target = PlayerManager.Instance.Player,
                    SummonID = EnemyEnums.EnemyID.渦巻ぐるぐる敵,
                    BulletData = new BulletStructs.NoBullet { }
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
                        Damage = 1
                    }
                };

            // 11
            case EnemyEnums.EnemyID.バリア突進中ボス:
                return new EnemyDataStructs.SummonEnemy
                {
                    ActionInterval = 9999.0f,
                    BulletInterval = 0.5f,
                    Target = PlayerManager.Instance.Player,
                    SummonID = EnemyEnums.EnemyID.渦巻ぐるぐる敵,
                    BulletData = new BulletStructs.SpreadEightShoot
                    {
                        MoveSpeed = 4.0f,
                        Acceleration = 0.5f,
                        SpriteType = SpriteData.SpriteType.EnemyBullet,
                        ColCategory = ColliderCategory.EnemyBullet,
                        Damage = 1
                    }
                };

            // 12
            case EnemyEnums.EnemyID.何もしない敵:
                return new EnemyDataStructs.NoAction
                {
                };

            // 13
            case EnemyEnums.EnemyID.プレイヤーのリングボムとして使う敵:
                return new EnemyDataStructs.NoAction
                { BulletData = new BulletStructs.NoBullet { } };
        }

        return null;
    }

}
