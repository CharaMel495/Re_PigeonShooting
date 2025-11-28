using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public static class CreateAddressableObject
{
    /// <summary>
    /// 敵のパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/EnemyTable")]
    public static void CreateEnemyTableAsset()
    {
        // newだと警告吐くので機能でインスタンス化
        var dataAsset = ScriptableObject.CreateInstance<EnemyParamTableAsset>();

        var dataReciever = JsonReader.LoadMonoParameterFromJson<EnemyParamTableReciever>(SummarizeResourceDirectory.ENEMYTABLE_PATH);

        // jsonデータをロード
        dataAsset.EnemyTable = new(dataReciever.EnemyTable);

        // アセット作成
        AssetDatabase.CreateAsset(dataAsset, SummarizeResourceDirectory.ENEMYTABLEASSET_PATH);

        // Asset作成後、反映させるために必要なメソッド
        AssetDatabase.Refresh();

        Debug.Log($"{SummarizeResourceDirectory.ENEMYTABLEASSET_PATH}にアセットが作成されました");
    }

    /// <summary>
    /// 一定時間ごとに起きるウェーブイベントのパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/WaveEventTable")]
    public static void CreateWaveEventTableAsset()
    {
        // newだと警告吐くので機能でインスタンス化
        var dataAsset = ScriptableObject.CreateInstance<WaveEventTableAsset>();

        var dataReciever = JsonReader.LoadMonoParameterFromJson<WaveEventTableReciever>(SummarizeResourceDirectory.WAVEEVENTTABLE_PATH);

        // jsonデータをロード
        dataAsset.WaveEventTable = new(dataReciever.WaveEventTable);

        // アセット作成
        AssetDatabase.CreateAsset(dataAsset, SummarizeResourceDirectory.WAVEEVENTTABLEASSET_PATH);

        // Asset作成後、反映させるために必要なメソッド
        AssetDatabase.Refresh();

        Debug.Log($"{SummarizeResourceDirectory.WAVEEVENTTABLEASSET_PATH}にアセットが作成されました");
    }

    /// <summary>
    /// 一定時間ごとに起きるウェーブイベントのパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/EasyStageCreate")]
    public static void CreateEasyWaveTimeTableAsset()
    {
        // newだと警告吐くので機能でインスタンス化
        var dataAsset = ScriptableObject.CreateInstance<WaveTimeTableAsset>();

        var dataReciever = JsonReader.LoadMonoParameterFromJson<WaveTimeTableReciever>(SummarizeResourceDirectory.WAVETIMETABLEEASY_PATH);

        // jsonデータをロード
        dataAsset.WaveTimeTable = new(dataReciever.WaveTimeTable);

        // アセット作成
        AssetDatabase.CreateAsset(dataAsset, SummarizeResourceDirectory.WAVETIMETABLEASSETEASY_PATH);

        // Asset作成後、反映させるために必要なメソッド
        AssetDatabase.Refresh();

        Debug.Log($"{SummarizeResourceDirectory.WAVETIMETABLEASSETEASY_PATH}にアセットが作成されました");
    }

    /// <summary>
    /// 一定時間ごとに起きるウェーブイベントのパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/NormalStageCreate")]
    public static void CreateNormalWaveTimeTableAsset()
    {
        // newだと警告吐くので機能でインスタンス化
        var dataAsset = ScriptableObject.CreateInstance<WaveTimeTableAsset>();

        var dataReciever = JsonReader.LoadMonoParameterFromJson<WaveTimeTableReciever>(SummarizeResourceDirectory.WAVETIMETABLENORMAL_PATH);

        // jsonデータをロード
        dataAsset.WaveTimeTable = new(dataReciever.WaveTimeTable);

        // アセット作成
        AssetDatabase.CreateAsset(dataAsset, SummarizeResourceDirectory.WAVETIMETABLEASSETNORMAL_PATH);

        // Asset作成後、反映させるために必要なメソッド
        AssetDatabase.Refresh();

        Debug.Log($"{SummarizeResourceDirectory.WAVETIMETABLEASSETNORMAL_PATH}にアセットが作成されました");
    }

    /// <summary>
    /// 一定時間ごとに起きるウェーブイベントのパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/HardStageCreate")]
    public static void CreateHardWaveTimeTableAsset()
    {
        // newだと警告吐くので機能でインスタンス化
        var dataAsset = ScriptableObject.CreateInstance<WaveTimeTableAsset>();

        var dataReciever = JsonReader.LoadMonoParameterFromJson<WaveTimeTableReciever>(SummarizeResourceDirectory.WAVETIMETABLEHARD_PATH);

        // jsonデータをロード
        dataAsset.WaveTimeTable = new(dataReciever.WaveTimeTable);

        // アセット作成
        AssetDatabase.CreateAsset(dataAsset, SummarizeResourceDirectory.WAVETIMETABLEASSETHARD_PATH);

        // Asset作成後、反映させるために必要なメソッド
        AssetDatabase.Refresh();

        Debug.Log($"{SummarizeResourceDirectory.WAVETIMETABLEASSETHARD_PATH}にアセットが作成されました");
    }
}

#endif