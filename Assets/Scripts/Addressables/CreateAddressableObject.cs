using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public static class CreateAddressableObject
{
    /// <summary>
    /// プレイヤーのパラメーターアセットを作る
    /// </summary>
    [MenuItem("ScriptableObjects/EnemyTable")]
    public static void CreateNPCTalkTableAsset()
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
}

#endif