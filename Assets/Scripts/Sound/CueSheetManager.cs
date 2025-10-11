using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CriWare;
using UnityEngine;

public enum BGM
{
    // BGMのキーをここに追加する
    MainStage,
    LastBoss,
    Tutorial,
    Ranking,
    Title,
    NormalBoss,
    None,
}

public enum SFX
{
    // SEのキーをここに追加する
    PlayerShot,
    AirBaster,
    IconButton,
    NormalButton,
    TutorialSuccess,
    TypeText,
    BatteryCharge,
    BossExplode,
    BulletHit,
    EnemyDefeat,
    EnemyDefeat2,
    BossExplosion2,
    Cutin,
    Powerup
}

public enum Voice
{
    ItemReserve_Voice,
    Heal_Voice,
    Beam_Voice,
    Ring_Voice,
    AirBaster_Voice,
    Dead_Voice,
    Start_Voice,
    BossDefeat_Voice,
    Result_Voice
}

/// <summary>
/// SE/BGM enumに対応するキューシート情報を保持・提供するクラス
/// </summary>
public class CueSheetManager
{
    /// <summary>
    /// キューシートを扱いやすくするためにクラスで単位化
    /// </summary>
    public class CueSheet
    {
        // キュー名
        public string Name
        { get; }
        // 使用するAcbファイル
        public CriAtomExAcb Acb
        { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">キューシート名</param>
        /// <param name="acb">使用するAcbファイル</param>
        public CueSheet(string name, CriAtomExAcb acb)
        {
            Name = name;
            Acb = acb;
        }
    }

    /// <summary>
    /// キューシートの連想配列
    /// </summary>
    private Dictionary<Enum, CueSheet> _cueSheetMap = new();
    /// <summary>
    /// 読み込んだキューシートのシート名
    /// </summary>
    private HashSet<string> _loadedCueSheetNames = new();

    /// <summary>
    /// すべてのSE・BGMに必要なキューシートを読み込む
    /// </summary>
    public async Task LoadAllCueSheetsAsync(CancellationToken token)
    {
        // BGM用のロード
        foreach (BGM bgm in Enum.GetValues(typeof(BGM)))
        {
            if (bgm == BGM.None) continue;
            await LoadCueSheetAsync(bgm, token);
        }

        // SE用のロード
        foreach (SFX se in Enum.GetValues(typeof(SFX)))
        {
            await LoadCueSheetAsync(se, token);
        }

        // SE用のロード
        foreach (Voice voice in Enum.GetValues(typeof(Voice)))
        {
            await LoadCueSheetAsync(voice, token);
        }
    }

    /// <summary>
    /// キューシートを非同期読み込みするメソッド
    /// </summary>
    /// <param name="key">読み込むサウンドのキー</param>
    /// <param name="token">非同期運用するためのトークン</param>
    private async Task LoadCueSheetAsync(Enum key, CancellationToken token)
    {
        // 受け取ったキーのEnum型の名前を取得(BGM, SFX等)
        string cueSheetName = GetCueSheetNameFromEnum(key);

        // 既に読み込んだキューシートに含まれているなら
        // 連想配列の登録だけして終了
        if (_loadedCueSheetNames.Contains(cueSheetName))
        {
            _cueSheetMap[key] = new CueSheet(cueSheetName, CriAtom.GetCueSheet(cueSheetName).acb);
            return;
        }

        // Acbファイルのパスを取得
        string acbPath = $"{SummarizeResourceDirectory.CRI_ACBFILE_PATH_TEMPLATE}{cueSheetName}.acb";
        string awbPath = $"{SummarizeResourceDirectory.CRI_ACBFILE_PATH_TEMPLATE}{cueSheetName}.awb";

        // キューシートを取り込む
        CriAtom.AddCueSheetAsync(null, acbPath, awbPath, null);
        // ロード完了まで待機
        await UniTask.WaitUntil(() => !CriAtom.CueSheetsAreLoading);
        // もしキャンセルリクエストが届いてたら例外を吐く
        token.ThrowIfCancellationRequested();

        // キューシートのAcbファイルを取得
        var acb = CriAtom.GetCueSheet(cueSheetName)?.acb;
        if (acb != null)
        {
            // 連想配列に登録
            _cueSheetMap[key] = new CueSheet(cueSheetName, acb);
            // 既に読み込んだリストに追加
            _loadedCueSheetNames.Add(cueSheetName);
        }
        else
        {
            Debug.LogWarning($"CueSheet '{cueSheetName}' の読み込みに失敗しました");
        }
    }

    /// <summary>
    /// キューシートを取得するメソッド
    /// </summary>
    /// <param name="key">欲しいサウンドのキー</param>
    /// <returns>要求されたサウンドが格納されたキューシート</returns>
    public CueSheet GetCueSheet(Enum key)
    {
        _cueSheetMap.TryGetValue(key, out var cueSheet);
        return cueSheet;
    }

    /// <summary>
    /// 渡されたサウンドのキーが含まれるキューシート名を返すメソッド
    /// </summary>
    /// <param name="key">調べたいサウンドのキー</param>
    /// <returns>そのサウンドが含まれるキューシート名</returns>
    private string GetCueSheetNameFromEnum(Enum key)
    {
        // Enumからクラス名（enumの型名）を取得して、それをCueSheet名とする
        return key.GetType().Name;
    }

    /// <summary>
    /// 全てのキューシートをアンロードするメソッド
    /// </summary>
    public void UnloadAllCueSheets()
    {
        foreach (var name in _loadedCueSheetNames)
        {
            CriAtom.RemoveCueSheet(name);
        }

        _loadedCueSheetNames.Clear();
        _cueSheetMap.Clear();
    }
}
