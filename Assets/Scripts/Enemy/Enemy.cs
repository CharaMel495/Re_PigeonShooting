using UnityEngine;
using EnemyEnums;

/// <summary>
/// 敵の本体クラス
/// (ただし、実体的にはデータ指向設計の為、概ねデータクラスである)
/// </summary>
public class Enemy : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    [Header("画像描画するやつ")]
    private SpriteRendererWrapper _renderer;

    [SerializeField]
    private Item _dropItem;

    [SerializeField]
    private ParticleController _particle;

    private Rect _playArea;

    /// <summary>
    /// カメラに映ってるか
    /// </summary>
    public bool IsInArea
        => _playArea.Contains(this.transform.position);

    /// <summary>
    /// 移動情報
    /// </summary>
    public EnemyDataStructs.IEnemyMoveData MoveData
    { get; set; }

    /// <summary>
    /// 行動情報
    /// </summary>
    public EnemyDataStructs.IEnemyActionData ActionData
    { get; set; }

    /// <summary>
    /// 現在アクティブか
    /// </summary>
    public bool IsActive
    { get; private set; } = false;

    /// <summary>
    /// 画面外の生存時間は5秒(300f)
    /// </summary>
    private const int _LIFETIME = 30;

    /// <summary>
    /// 残り生存期間
    /// </summary>
    public int RemainLifeTime
    { get; private set; }

    /// <summary>
    /// 破棄待ちフラグ
    /// </summary>
    public bool IsDestroyWaiting
    { get; set; }

    public string Name
    { get; private set; }

    public ICollider Collider
    { get; set; }

    private const float _INVINCIBLETIME = 0.1f;

    private bool _isInvincible = false;

    private Timer _timer;

    public int Life
    { get; set; }

    public int Score
    { get; set; }

    public object TriggerEnterEventData 
        => new DamageEventData { Damage = HasDamage };

    public object TriggerStayEventData 
        => null;

    public object TriggerExitEventData 
        => null;

    private bool _isDestroyedByShot = true;

    public bool IsNotDamagedEnemy
    { get; set; }

    public int HasDamage
    { get; set; }

    public void Initialize(Rect playArea)
    {
        _renderer.Initialize();
        _renderer.SetEnabled(false);
        _timer = new();
        _timer.Initialize();
        _playArea = playArea;
        IsNotDamagedEnemy = false;
    }

    public void EnActive(Sprite sprite, string name)
    {
        if (_renderer.CurrentSprite != sprite)
            _renderer.SetSprite(sprite);
        _renderer.SetEnabled(true);
        this.transform.parent = null;
        RemainLifeTime = _LIFETIME;
        IsActive = true;
        IsDestroyWaiting = false;
        Name = name;
        _isDestroyedByShot = true;

        EventDispatcher.Instance.Bind(this, Name);

        if (MoveData is EnemyDataStructs.MissileMove)
            this.transform.up = MoveData.MoveDir;

        IsNotDamagedEnemy = false;
        HasDamage = 1;
    }

    private void FixedUpdate()
    {
        if (!IsActive)
            return;

        _timer.Update();

        if (MoveData != null)
            MoveData.ElaspedTime += Time.fixedDeltaTime;

        if (_isInvincible)
        {
            if (_renderer.GetSpriteAlpha() < 0.1f)
                _renderer.SetSpriteAlpha(1.0f);
            else
                _renderer.SetSpriteAlpha(0.0f);
        }
        else if (_renderer.GetSpriteAlpha() < 0.1f)
            _renderer.SetSpriteAlpha(1.0f);

        if (ActionData.BulletData == null)
            return;

        var bulletData = ActionData.BulletData;
        bulletData.Origin = this.transform.position;
    }

    public void SetActionInterval()
        => ActionData.CurrentInterval = ActionData.ActionInterval;

    public void AdvanceInterval()
        => ActionData.CurrentInterval -= Time.fixedDeltaTime;

    public void OutOfView()
    {
        --RemainLifeTime;

        if (RemainLifeTime < 1)
            IsDestroyWaiting = true;
    }

    [CallableEvent("OnTriggerEnter")]
    public void OnHit(object data)
    {
        if (_isInvincible || IsNotDamagedEnemy)
            return;

        if (!(data is DamageEventData damageData))
            return;
        else
        {
            _isInvincible = true;

            Life -= damageData.Damage;

            _isDestroyedByShot = damageData.Damage < 10;
        }

        if (Life < 1)
        {
            IsDestroyWaiting = true;
            EventDispatcher.Instance.Dispatch("AddScore", Score);
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Shot_SmashEnemy);
            return;
        }

        _timer.CreateTask(() => _isInvincible = false, _INVINCIBLETIME);
    }

    public void Destroy(Transform poolRoot, bool isItemDroppable)
    {
        _renderer.SetEnabled(false);
        this.transform.parent = poolRoot;
        RemainLifeTime = _LIFETIME;
        IsActive = false;
        IsDestroyWaiting = false;
        _timer.Initialize();
        _isInvincible = false;
        EventDispatcher.Instance.Unbind(this, Name);
        this.transform.localScale = Vector3.one;
        this.transform.rotation = Quaternion.identity;
        _particle.PlayParticle();

        int rand = Random.Range(0, 100);
        
        
        ItemType itemType = GetRandomItemType();

        // アイテムをドロップできるなら
        if (isItemDroppable && itemType != ItemType.None)
        {
            var item = Instantiate(_dropItem, this.transform.position, Quaternion.identity);
            item.Initialize(itemType);
            item.Dir = this.MoveData.MoveDir;
        }

        if (_isDestroyedByShot && Life < 1)
        {
            if (itemType == ItemType.None)
                CRISoundManager.Instance.PlaySE(SFX.EnemyDefeat);
            else
                CRISoundManager.Instance.PlaySE(SFX.EnemyDefeat2);
        }

        if (this.transform.childCount < 1)
            return;

        int count = this.transform.childCount;

        for (int i = 0; i < count; ++i)
        {
            var child = this.transform.GetChild(i).gameObject;
            var enemy = child.GetComponent<Enemy>();
            if (enemy != null)
                enemy.IsDestroyWaiting = true;
        }

        ItemType GetRandomItemType()
        {
            int rand = Random.Range(0, 100);

            if (Life > 0)
                return ItemType.None;

            if (rand < 20)
                return ItemType.Garbage;
            else
                return ItemType.EXP;
        }
    }
}
