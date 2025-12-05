using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// ボスの抽象クラス
/// </summary>
public abstract class BossBase : MonoBehaviour, IColliderbleObject
{
    [SerializeField]
    protected Item _itemPrefab;

    [SerializeField]
    protected ImageWrapper _lifeImage;

    [SerializeField]
    protected ParticleSystem _smashedParticle;
    // やられた時に、パーティクルを出す回数
    protected int _effectCount = 50;

    [SerializeField]
    protected SpriteRendererWrapper _renderer;
    // ダメージを受けたときに点滅させる時間
    protected float _falshTime = 0.1f;

    public ICollider Collider 
    { get; protected set; }

    public bool IsDestroyWaiting
    { get; set; }

    public object TriggerEnterEventData
    { get; set; }

    public object TriggerStayEventData 
    { get; set; }

    public object TriggerExitEventData
    { get; set; }

    // 行動可能かを表す
    public bool IsActionable
    { get; protected set; }
    public bool IsMovable
    { get; protected set; }

    // ボスの名前
    public string BossName
    { get; set; }

    public int Life
    { get; set; }

    public int Score
    { get; set; }

    public EnemyDataStructs.IEnemyMoveData MoveData
    { get; set; }

    protected Timer _timer;
    protected int _timerTaskID = -1;

    public abstract void Initialize();
    public abstract void Smashed();
    public abstract void Action();
    public abstract void OnHit(object data);

    public virtual void StartAction(object data)
    {
        IsActionable = true;
        IsMovable = true;
    }

    public void PlaySmashedEffect()
    {
        Vector2 rnd = Random.insideUnitCircle * 3.0f;
        var center = this.transform.position;
        Vector3 pos = new Vector3(center.x + rnd.x, center.y + rnd.y, center.z);
        var particle = Instantiate(_smashedParticle, pos, Quaternion.identity);
        CRISoundManager.Instance.PlaySE(SFX.BossExplosion2);

        --_effectCount;

        if (_effectCount > 0)
            _timer.CreateTask(PlaySmashedEffect, 0.1f);
        else
        {
            CRISoundManager.Instance.PlaySE(SFX.BossExplode);
            _renderer.SetSpriteAlpha(0.0f);
            IsDestroyWaiting = true;
        }
    }

    public void DestroyByColliderManager()
    {
        ColliderManager.Instance.RemoveCollider(Collider);
        Destroy(this.gameObject);
    }

    protected void ThrowItem(ItemType type)
    {
        var item = Instantiate(_itemPrefab, this.transform.position, Quaternion.identity);
        item.Initialize(type);
        item.Dir = RandomVector3();
        item.AddtionalPower = 7.5f;

        Vector3 RandomVector3()
        {
            Vector3 v = Vector3.zero;
            v.x = UnityEngine.Random.Range(-1.0f, 1.0f);
            v.y = UnityEngine.Random.Range(-1.0f, 1.0f);
            v.z = 0;
            return v.normalized;
        }
    }

    protected void UpdateLifeUI(float ratio)
    {
        // 0～1で clamping
        ratio = Mathf.Clamp01(ratio);

        // ゲージの進行
        _lifeImage.SetFillAmount(ratio);

        // 色の変化（緑→黄→赤）
        Color newColor;
        if (ratio > 0.5f)
        {
            // 緑→黄（0.5～1.0）
            float t = (ratio - 0.5f) * 2f;
            newColor = Color.Lerp(Color.yellow, Color.green, t);
        }
        else
        {
            // 黄→赤（0.0～0.5）
            float t = ratio * 2f;
            newColor = Color.Lerp(Color.red, Color.yellow, t);
        }

        _lifeImage.SetImageColor(newColor);
    }
}
