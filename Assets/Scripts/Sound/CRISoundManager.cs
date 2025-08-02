using UnityEngine;
using CriWare;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

/// <summary>
/// CRIWARE音声を統括するサウンドマネージャー（BGM/SE管理）
/// </summary>
public class CRISoundManager : SingletonMonoBehaviour<CRISoundManager>
{
    /// <summary>
    /// BGMの再生を担うプレイヤー
    /// </summary>
    private CriAtomExPlayer _bgmPlayer;

    /// <summary>
    /// 立体音響を使わない(SEを鳴らすときに使用する音源の指定がない)
    /// 場合に代わりにSEを鳴らす
    /// </summary>
    private CriAtomSource _defaultSeSource;

    /// <summary>
    /// キューシート管理クラス
    /// </summary>
    private CueSheetManager _cueSheetManager;

    /// <summary>
    /// 現在再生しているBGMのAcbファイル
    /// </summary>
    private CriAtomExAcb _currentBgmAcb;

    /// <summary>
    /// 現在流れているBGMを記憶する変数
    /// </summary>
    private BGM _currentBgm = BGM.None;

    /// <summary>
    /// 非同期処理を多用するので、バグ避けのトークン
    /// </summary>
    private CancellationToken _destroyToken;

    // マスターボリューム
    public static float MasterVolume { get; set; } = 1f;
    // BGMボリューム
    public static float BGMVolume { get; set; } = 0.5f;
    // SEボリューム
    public static float SEVolume { get; set; } = 0.75f;

    private void FixedUpdate()
    {
        //_cueSheetManager?.Update();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public async UniTaskVoid Initialize()
    {
        // オブジェクト破棄時に停止されるようにトークンを取得
        _destroyToken = this.GetCancellationTokenOnDestroy();

        // BGMを鳴らすクラスを生成
        _bgmPlayer = new CriAtomExPlayer();
        _bgmPlayer.Loop(true);
        _bgmPlayer.SetVolume(BGMVolume * MasterVolume);

        // SEを鳴らすクラスを生成
        _defaultSeSource = gameObject.AddComponent<CriAtomSource>();
        gameObject.AddComponent<CriAtom>();

        // Acfファイルを登録
        CriAtomEx.RegisterAcf(null, SummarizeResourceDirectory.CRI_ACFFILE_PATH);

        // キューシート管理クラスを生成
        _cueSheetManager = new CueSheetManager();
        // 使用する全てのキューシートを読み込む
        await _cueSheetManager.LoadAllCueSheetsAsync(_destroyToken);

        ChangeMasterVolume(0.5f);
        ChangeBGMVolume(0.25f);
        ChangeSEVolume(1.0f);
        PlayBGM(BGM.MainStage);
    }

    /// <summary>
    /// SEを鳴らすメソッド
    /// </summary>
    /// <param name="se">鳴らしたい音源のキー</param>
    public void PlaySE(SFX se)
    {
        PlaySE(se, _defaultSeSource);
    }

    /// <summary>
    /// SEを鳴らすメソッド
    /// </summary>
    /// <param name="se">鳴らしたい音源のキー</param>
    /// <param name="source">音源の再生元</param>
    public void PlaySE(SFX se, CriAtomSource source)
    {
        // キューシートを取得
        var cueSheet = _cueSheetManager.GetCueSheet(se);
        if (cueSheet == null) 
            return;

        // 鳴らすキューを文字列で指定し再生
        source.cueSheet = cueSheet.Name;
        source.Play(se.ToString());
    }

    /// <summary>
    /// BGMを流すメソッド
    /// </summary>
    /// <param name="bgm">流したいBGMのキー</param>
    public void PlayBGM(BGM bgm)
    {
        // もし既に流れているBGMならここで終わる
        if (_currentBgm == bgm || bgm == BGM.None) 
            return;

        // BGMのキューシートを取得
        var cueSheet = _cueSheetManager.GetCueSheet(bgm);
        if (cueSheet == null) 
            return;

        // Acbファイルを登録
        _currentBgmAcb = cueSheet.Acb;
        _currentBgm = bgm;

        // 現在流れている曲を停止
        _bgmPlayer.Stop();
        // キューをセット
        _bgmPlayer.SetCue(_currentBgmAcb, bgm.ToString());
        // 再生開始
        _bgmPlayer.Start();
    }

    /// <summary>
    /// BGMの再生を停止するメソッド
    /// </summary>
    public void StopBGM()
    {
        _bgmPlayer?.Stop();
        _currentBgm = BGM.None;
    }

    /// <summary>
    /// BGMの再生を中断するメソッド
    /// </summary>
    public void PauseBGM()
    {
        _bgmPlayer?.Pause();
    }

    /// <summary>
    /// 中断していたBGMを再開させるメソッド
    /// </summary>
    public void ResumeBGM()
    {
        _bgmPlayer?.Resume(CriAtomEx.ResumeMode.PausedPlayback);
    }

    /// <summary>
    /// BGM音量を変更するメソッド
    /// </summary>
    /// <param name="volume">変更後の音量</param>
    public void ChangeBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
        _bgmPlayer?.SetVolume(BGMVolume * MasterVolume);
    }

    /// <summary>
    /// SE音量を変更するメソッド
    /// </summary>
    /// <param name="volume">変更後の音量</param>
    public void ChangeSEVolume(float volume)
    {
        SEVolume = Mathf.Clamp01(volume);
        _defaultSeSource.volume = SEVolume * MasterVolume;
    }

    /// <summary>
    /// マスター音量を変更するメソッド
    /// </summary>
    /// <param name="volume">変更後の音量</param>
    public void ChangeMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        ChangeBGMVolume(BGMVolume);
        ChangeSEVolume(SEVolume);
    }

    /// <summary>
    /// BGMの再生速度を変更するメソッド
    /// </summary>
    /// <param name="speed">新たな再生速度</param>
    public void ChangePlaySpeed(float speed)
    {
        _bgmPlayer.SetPlaybackRatio(speed);
    }

    /// <summary>
    /// BGMの再生速度をリセットするメソッド
    /// </summary>
    public void ResetPlaySpeed()
    {
        _bgmPlayer.SetPlaybackRatio(1.0f);
    }
}
