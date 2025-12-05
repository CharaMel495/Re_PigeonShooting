using UnityEngine;
using DG.Tweening;

public class PlayerFollowCamera : MonoBehaviour
{
    [Header("追跡対象")]
    [SerializeField]
    private Transform _followTarget;

    [Header("ボス出現時演出周りの設定")]
    [SerializeField]
    private float _bossCaptureTime;
    [SerializeField]
    private float _bossCaptureSize;
    [SerializeField]
    private float _bossNameViewTime;
    [SerializeField]
    private float _zoomOutTime;
    [SerializeField]
    private float _bossBattleCamSize;
    [SerializeField]
    private Vector2 _bossBattlePadding;

    private float _halfWidth;
    private float _halfHeight;

    private Vector3 _shakeOffset;
    private float _shakeTime;
    private float _shakePower;

    private bool _isFollowPlayer;

    public void Initialize()
    {
        var cam = GetComponent<Camera>();
        _halfHeight = cam.orthographicSize;
        _halfWidth = _halfHeight * cam.aspect;
        EventDispatcher.Instance.Bind(this);
        _isFollowPlayer = true;
    }

    private void Update()
    {
        if (_isFollowPlayer)
            Follow();
        UpdateShake();
    }

    private void Follow()
    {
        Vector3 pos = _followTarget.position;
        float posZ = transform.position.z;

        Correct(ref pos); // プレイヤー位置をステージ内でClamp
        pos.z = posZ;

        // シェイクオフセットを最後に適用
        transform.position = pos + _shakeOffset;
    }

    private void Correct(ref Vector3 pos)
    {
        var playArea = StageManager.Instance.PlayArea;

        pos.x = Mathf.Clamp(pos.x,
            playArea.xMin + _halfWidth,
            playArea.xMax - _halfWidth);

        pos.y = Mathf.Clamp(pos.y,
            playArea.yMin + _halfHeight,
            playArea.yMax - _halfHeight);
    }

    /// <summary>
    /// カメラを揺らす（duration: 持続時間, power: 揺れ幅）
    /// </summary>
    public void Shake(float duration, float power)
    {
        _shakeTime = duration;
        _shakePower = power;
    }

    private void UpdateShake()
    {
        if (_shakeTime > 0f)
        {
            _shakeTime -= Time.deltaTime;
            _shakeOffset = (Vector3)Random.insideUnitCircle * _shakePower;
        }
        else
        {
            _shakeOffset = Vector3.zero;
        }
    }

    [CallableEvent("BossEvent")]
    public void WhenBossAppeared(object data)
    {
        // ボスを出現させる
        EventDispatcher.Instance.Dispatch("AppearBoss", data);

        // 入力をプレイヤーから奪う
        InputManager.Instance.ChangeInputHandler(InputHandler.UI);

        // ボスをクローズアップ
        var pos = EnemyManager.Instance.CurrentBoss.transform.position;
        pos.z = -10;
        this.transform.DOMove(pos, _bossCaptureTime).SetEase(Ease.InCubic).
            OnComplete(ViewBossName);
        var cam = GetComponent<Camera>();
        cam.DOOrthoSize(_bossCaptureSize, _bossCaptureTime);

        _isFollowPlayer = false;

        void ViewBossName()
        {
            // ボスの名前をUIに表示
            EventDispatcher.Instance.Dispatch("ViewBossName", EnemyManager.Instance.CurrentBoss.BossName);

            var cam = GetComponent<Camera>();
            cam.DOOrthoSize(_bossCaptureSize + 1.0f, _bossNameViewTime).
                OnComplete(StartBossBattle);
        }

        void StartBossBattle()
        {
            InputManager.Instance.ReturnHandle();

            PlayerManager.Instance.Player.OnGetItem(new GetItemEventData { Value = 10, ItemType = ItemType.Battery_Green });

            var cam = GetComponent<Camera>();
            cam.DOOrthoSize(_bossBattleCamSize, _zoomOutTime).
                OnComplete(() => StageManager.Instance.SetBossBattlePlayArea(GetCameraWorldRect(cam)));

            // 現在映しているカメラ領域から戦闘エリア矩形を作成する
            Rect GetCameraWorldRect(Camera cam)
            {
                float height = cam.orthographicSize * 2f;
                float width = height * cam.aspect;

                float left = cam.transform.position.x - width / 2f + _bossBattlePadding.x;
                float bottom = cam.transform.position.y - height / 2f + _bossBattlePadding.y;

                // ボス戦開始
                EventDispatcher.Instance.Dispatch("StartBossBattle");

                // パディングを×２してるのは、両端それぞれにパディングをかけるから
                return new Rect(left, bottom, width - _bossBattlePadding.x * 2, height - _bossBattlePadding.y * 2);
            }

        }
    }
}
