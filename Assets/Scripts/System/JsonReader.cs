using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Jsonファイルを読み込む関数をまとめたクラス
/// </summary>
public static class JsonReader
{
    /// <summary>
    /// 単体のパラメータを読み込む関数
    /// </summary>
    /// <typeparam name="T">読み込みたいデータ型</typeparam>
    /// <param name="path">ファイルパス</param>
    public static T LoadMonoParameterFromJson<T>(string path)
    {
        StreamReader rd;

        //パスからデータを持って来れるかを確認する
        try
        {
            rd = new(Application.dataPath + path);
        }
        catch (FileNotFoundException e)
        {
            //もし存在しないパスならログに書いて終了
            Debug.LogError($"ファイル名{e.FileName}が見つかりませんでした");
            return default;
        }

        string json = rd.ReadToEnd();

        rd.Close();

        return JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// パラメータテーブルを読み込む関数
    /// </summary>
    /// <typeparam name="T">読み込みたいデータ型</typeparam>
    /// <param name="path">ファイルパス</param>
    public static T LoadParameterTableFromJson<T>(string path)
    {
        string fileData = "";

        try
        {
            fileData = File.ReadAllText(Application.dataPath + path);
        }
        catch (FileNotFoundException e)
        {
            //もし存在しないパスならログに書いて終了
            Debug.LogError($"ファイル名{e.FileName}が見つかりませんでした");
            return default;
        }

        T table = JsonUtility.FromJson<T>(fileData);

        return table;
    }
}
