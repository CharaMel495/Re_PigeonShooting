using UnityEngine;

public class EnemyMoveParameterCreator
{
    public EnemyDataStructs.IEnemyMoveData GetEnemyMoveData(EnemyEnums.EnemyID moveType)
    {
        switch (moveType)
        {
            // 0
            case EnemyEnums.EnemyID.キホンの雑魚敵:
                return new EnemyDataStructs.TrackPlayer
                {
                    MoveSpeed = 3.0f,
                    Target = PlayerManager.Instance.Player,
                    TurnRate = 80.0f,
                };

            // 1
            case EnemyEnums.EnemyID.キホンの弾を撃つ敵:
                return new EnemyDataStructs.StopPointMove
                {
                    MoveSpeed = 2.0f,
                    StopThreshold = 0.1f,
                    NextMove = new EnemyDataStructs.StopPointMove()
                };

            // 2
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵:
                return new EnemyDataStructs.MissileMove
                {
                    MoveSpeed = 5.0f,
                    SecondMoveSpeed = 3.0f,
                    MaxMoveSpeed = 20.0f,
                    Acceleration = 0.15f,
                    DisAcceleration = -2.0f,
                    Target = PlayerManager.Instance.Player,
                    TurnRate = 80.0f,
                    IsStraight = false,
                };

            // 3
            case EnemyEnums.EnemyID.プレイヤーに突っ込んでくるミサイル敵_弾あり:
                return new EnemyDataStructs.MissileMove
                {
                    MoveSpeed = 5.0f,
                    SecondMoveSpeed = 3.0f,
                    MaxMoveSpeed = 20.0f,
                    Acceleration = 1.0f,
                    DisAcceleration = -2.0f,
                    Target = PlayerManager.Instance.Player,
                    TurnRate = 80.0f,
                    IsStraight = false,
                };

            // 4
            case EnemyEnums.EnemyID.ミサイル敵を撃つ敵:
                return new EnemyDataStructs.StopPointMove
                {
                    MoveSpeed = 2.0f,
                    StopThreshold = 0.1f,
                    NextMove = new EnemyDataStructs.StopPointMove()
                };

            // 5
            case EnemyEnums.EnemyID.輪っか弾を撃つ敵:
                return new EnemyDataStructs.StopPointMove
                {
                    MoveSpeed = 2.0f,
                    StopThreshold = 0.1f,
                    NextMove = new EnemyDataStructs.StopPointMove()
                };

            // 6
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵:
                return new EnemyDataStructs.SlavedSpiralMove
                {
                    Acceleration = 0,
                    MoveSpeed = 100.0f,
                };

            // 7
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵_自機狙い単発弾:
                return new EnemyDataStructs.SlavedSpiralMove
                {
                    Acceleration = 0,
                    MoveSpeed = 30.0f,
                };

            // 8
            case EnemyEnums.EnemyID.渦巻ぐるぐる敵_後方3way:
                return new EnemyDataStructs.SlavedSpiralMove
                {
                    Acceleration = 0,
                    MoveSpeed = 100.0f,
                };

            // 9
            case EnemyEnums.EnemyID.バリア突進敵:
                return new EnemyDataStructs.TrackPlayer
                {
                    MoveSpeed = 3.0f,
                    Target = PlayerManager.Instance.Player,
                    TurnRate = 80.0f,
                };

            // 10
            case EnemyEnums.EnemyID.レーザー発射敵:
                return new EnemyDataStructs.NoMove
                {

                };

            // 11
            case EnemyEnums.EnemyID.バリア突進中ボス:
                return new EnemyDataStructs.TrackPlayer
                {
                    MoveSpeed = 2.0f,
                    Target = PlayerManager.Instance.Player,
                    TurnRate = 80.0f,
                };

            // 12
            case EnemyEnums.EnemyID.何もしない敵:
                return new EnemyDataStructs.NoMove
                {
                };
        }


        return null;
    }
}
