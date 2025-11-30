using UnityEngine;
using static UnityEngine.ParticleSystem;

/// <summary>
/// ボスの抽象クラス
/// </summary>
public abstract class BossBase : MonoBehaviour, IColliderbleObject
{
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
    public abstract void OnHit();

    public void StartAction(object data)
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
            IsDestroyWaiting = true;
        }
    }

    public void DestroyByColliderManager()
    {
        ColliderManager.Instance.RemoveCollider(Collider);
        Destroy(this.gameObject);
    }
}
