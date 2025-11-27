using BulletStructs;
using UnityEngine;

public enum DebugMenu
{
    へにょりテスト,
    この葉隠れテスト,
    画面反射弾テスト,
    十字架レーザー弾テスト
}

public class DebugPlayer : MonoBehaviour
{
    [SerializeField]
    private DebugMenu _debugMenu;

    private float _fireInterval;
    private float _rotateSpeed;
    private float _counter;
    private float _switchTime;

    private Timer _timer;

    private void Start()
    {
        _timer = new();
        _timer.Initialize();

        _fireInterval = 0.2f;
        _rotateSpeed = 5.0f;

        if (_debugMenu == DebugMenu.この葉隠れテスト)
            _timer.CreateTask(KeepingFireRSpin, _fireInterval);
    }

    private void Update()
    {
        if (InputManager.CheckKey(InputManager.DesideKey, InputHandler.Player))
            ExecuteDebugCommand();
    }

    private void FixedUpdate()
    {
        _timer.Update();
    }

    private void ExecuteDebugCommand()
    {
        switch (_debugMenu)
        {
            case DebugMenu.へにょりテスト:

                FireSlopeBullet();

                break;

            case DebugMenu.画面反射弾テスト:

                FireBounceBullet();

                break;

            case DebugMenu.十字架レーザー弾テスト:

                FireCrossLazer();

                break;
        }
    }

    private void FireSlopeBullet()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(BulletStructs.CurveAndStraight), 0);

        shootData.Dir = RandomVector3();

        BulletManager.Instance.Shooter.Shoot(shootData);

        Vector3 RandomVector3()
        {
            Vector3 v = Vector3.zero;
            v.x = Random.Range(-1.0f, 1.0f);
            v.y = Random.Range(-1.0f, 1.0f);
            v.z = 0;
            return v.normalized;
        }
    }

    private void FireBounceBullet()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(BulletStructs.MultiBounce), 0);

        shootData.Dir = RandomVector3();

        BulletManager.Instance.Shooter.Shoot(shootData);

        Vector3 RandomVector3()
        {
            Vector3 v = Vector3.zero;
            v.x = Random.Range(-1.0f, 1.0f);
            v.y = Random.Range(-1.0f, 1.0f);
            v.z = 0;
            return v.normalized;
        }
    }

    private void FireCrossLazer()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(CrossLazerParam), 0);

        shootData.Dir = RandomVector3();

        BulletManager.Instance.CreateCrossLazer((CrossLazerParam)shootData);

        Vector3 RandomVector3()
        {
            Vector3 v = Vector3.zero;
            v.x = Random.Range(-1.0f, 1.0f);
            v.y = Random.Range(-1.0f, 1.0f);
            v.z = 0;
            return v.normalized;
        }
    }

    public void KeepingFireRSpin()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(BulletStructs.MultiCurve), 1);

        shootData.Dir = new(Mathf.Cos(Time.fixedTime * _rotateSpeed), Mathf.Sin(Time.fixedTime * _rotateSpeed), 0.0f);
        shootData.Dir.Normalize();

        BulletManager.Instance.Shooter.Shoot(shootData);

        _timer.CreateTask(KeepingFireRSpin, _fireInterval);
    }

    public void KeepingFireLSpin()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(BulletStructs.MultiCurve), 0);

        shootData.Dir = new(Mathf.Cos(-Time.fixedTime * _rotateSpeed), Mathf.Sin(-Time.fixedTime * _rotateSpeed), 0.0f);
        shootData.Dir.Normalize();

        BulletManager.Instance.Shooter.Shoot(shootData);

        _timer.CreateTask(KeepingFireLSpin, _fireInterval);
    }

    public void KeepingRingFireSpin()
    {
        var shootData = BulletManager.Instance.DataBase.GetBulletData(typeof(BulletStructs.BounceRing), 0);

        shootData.Dir = new(Mathf.Cos(-Time.fixedTime * _rotateSpeed), Mathf.Sin(-Time.fixedTime * _rotateSpeed), 0.0f);
        shootData.Dir.Normalize();

        BulletManager.Instance.Shooter.Shoot(shootData);

        _timer.CreateTask(KeepingRingFireSpin, _fireInterval);
    }
}
