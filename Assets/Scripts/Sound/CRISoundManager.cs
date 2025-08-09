using UnityEngine;
using CriWare;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

/// <summary>
/// CRIWARE音声を統括するサウンドマネージャー（BGM/SE管理）
/// </summary>
public class CRISoundManager : MonoBehaviour
{
    private static CRISoundManager _instance;

    /// <summary>
    /// シングルトンインスタンス
    /// </summary>
    public static CRISoundManager Instance => _instance;

    /// <summary>
    /// BGMの再生を担うプレイヤー
    /// </summary>
    private CriAtomExPlayer _bgmPlayer;
    /// <summary>
    /// BGMの再生中に制御を行う構造体
    /// </summary>
    private CriAtomExPlayback _bgmPlayback;

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

    private BGM _registingBGM = BGM.None;

    /// <summary>
    /// 非同期処理を多用するので、バグ避けのトークン
    /// </summary>
    private CancellationToken _destroyToken;

    // マスターボリューム
    public static float MasterVolume { get; set; } = 1.0f;
    // BGMボリューム
    public static float BGMVolume { get; set; } = 1.0f;
    // SEボリューム
    public static float SEVolume { get; set; } = 1.0f;

    private Durator _durator;

    private int _effectTask;

    private float _volumeChache = -1.0f;

    private static bool _isInitialized = false;

    // ==== 安全装置：SEプール & スロットル ====
    private int _sePoolSize = 16;           // 同時SEの上限
    private CriAtomSource[] _sePool;
    private int _seIndex = 0;

    // 連打間引き（SEごとに最短インターバル）
    private float _seThrottleInterval = 0.04f; // 40ms
    private readonly System.Collections.Generic.Dictionary<SFX, float> _nextPlayable
        = new System.Collections.Generic.Dictionary<SFX, float>();

    // 時刻取得は unscaledTime を推奨（TimeScale 0 演出中でも動く）
    private float Now => Time.unscaledTime;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        //_cueSheetManager?.Update();
        _durator?.Update();

        if (_registingBGM != BGM.None)
        {
            var bgm = _registingBGM;
            _registingBGM = BGM.None;
            PlayBGM(bgm);
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public async UniTask Initialize()
    {
        if (_isInitialized)
        {
            
            return;
        }

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

        ChangeMasterVolume(MasterVolume);
        ChangeBGMVolume(BGMVolume);
        ChangeSEVolume(SEVolume);

        _durator = new();
        _durator.Initialize();

        // --- SEを鳴らすクラスを生成（プール化） ---
        _sePool = new CriAtomSource[_sePoolSize];
        for (int i = 0; i < _sePoolSize; i++)
        {
            var src = gameObject.AddComponent<CriAtomSource>();
            src.volume = SEVolume * MasterVolume;
            _sePool[i] = src;
        }
        // 後方互換：既存API用の参照（最初のプールを流用）
        _defaultSeSource = _sePool[0];


        _isInitialized = true;
    }

    // 既存の PlaySE(SFX se) を差し替え（間引き＋プール）
    public void PlaySE(SFX se)
    {
        if (!_isInitialized)
            return;

        // スロットル：短時間の多重発火を抑制
        if (_nextPlayable.TryGetValue(se, out var t) && Now < t) return;
        _nextPlayable[se] = Now + _seThrottleInterval;

        // プールから次のソースを取得
        var src = GetNextSeSource();
        if (src == null) return; // ありえないけど念のため

        var cueSheet = _cueSheetManager.GetCueSheet(se);
        if (cueSheet == null) return;

        src.cueSheet = cueSheet.Name;
        src.volume = SEVolume * MasterVolume; // 念のため直前反映
        src.Play(se.ToString());
    }

    // 明示ソース版は残す（ピンポイント再生用）
    public void PlaySE(SFX se, CriAtomSource source)
    {
        var cueSheet = _cueSheetManager.GetCueSheet(se);
        if (cueSheet == null) return;

        source.cueSheet = cueSheet.Name;
        source.volume = SEVolume * MasterVolume;
        source.Play(se.ToString());
    }

    private CriAtomSource GetNextSeSource()
    {
        // ラウンドロビンで使い回し（多重再生は各ソースが担当）
        var src = _sePool[_seIndex];
        _seIndex = (_seIndex + 1) % _sePool.Length;
        return src;
    }


    /// <summary>
    /// BGMを流すメソッド
    /// </summary>
    /// <param name="bgm">流したいBGMのキー</param>
    public void PlayBGM(BGM bgm)
    {
        if (!_isInitialized)
            _registingBGM = bgm;

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
        _bgmPlayback = _bgmPlayer.Start();
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

        _bgmPlayer.SetVolume(BGMVolume * MasterVolume);
        _bgmPlayer.Update(_bgmPlayback);
    }

    public void ChangeSEVolume(float volume)
    {
        SEVolume = Mathf.Clamp01(volume);
        float v = SEVolume * MasterVolume;

        // すべてのSEソースへ反映（プール全体）
        if (_sePool != null)
        {
            for (int i = 0; i < _sePool.Length; i++)
            {
                if (_sePool[i] != null) _sePool[i].volume = v;
            }
        }
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

    public void BombEffect(float duration)
    {
        if (_durator.IsTaskExistTask(_effectTask))
            _durator.CanncellTask(_effectTask, true);

        // キャッシュが無ければ記憶
        if (_volumeChache < 0)
            _volumeChache = BGMVolume;

        ChangeBGMVolume(0.0f);

        _effectTask = _durator.CreateTask((float _elapsedTime, float _endTime) => ZeroToCurrent(_elapsedTime, _endTime, _volumeChache),
            () => { ChangeBGMVolume(_volumeChache); _volumeChache = -1.0f; }, duration);
    }

    public void ZeroToCurrent(float _elapsedTime, float _endTime, float volume)
    {
        var ratio = _elapsedTime / _endTime;

        ChangeBGMVolume(BGMVolume + Time.fixedDeltaTime);
    }
}
