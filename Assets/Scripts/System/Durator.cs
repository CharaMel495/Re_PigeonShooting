using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 持続的に実行したい処理を預けて実行する関数
/// </summary>
public class Durator
{
    /// <summary>
    /// 持続処理の終了時に呼び出したい処理
    /// </summary>
    public delegate void CallBackTask();

    /// <summary>
    /// 返り値がvoidである関数デリゲート
    /// </summary>
    public delegate void DurationTask(float _elapsedTime, float _endTime);

    /// <summary>
    /// 渡されたタスクの情報をまとめた構造体
    /// </summary>
    public class TaskStruct
    {
        /// <summary>
        /// 経過時間
        /// </summary>
        public float Timer;
        /// <summary>
        /// 継続時間
        /// </summary>
        public float DurationTime;
        /// <summary>
        /// 持続的に実行したい関数
        /// </summary>
        public DurationTask Func;
        /// <summary>
        /// 持続終了時に実行したい関数
        /// </summary>
        public CallBackTask CallBack;
        /// <summary>
        /// 現在計測中か
        /// (関数の実行待ちか)
        /// </summary>
        public bool IsMeasuring;
        /// <summary>
        /// TimeScaleを無視するか
        /// </summary>
        public bool OnUnscaledTime;
        /// <summary>
        /// タイマーを進める関数
        /// </summary>
        public void CountTimer()
        {
            Timer += OnUnscaledTime ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime;
        }
        /// <summary>
        /// 計測フラグを立てる関数
        /// </summary>
        public void StartMeasuring()
            => IsMeasuring = true;
        /// <summary>
        /// 計測フラグを折る関数
        /// </summary>
        public void StopMeasuring()
            => IsMeasuring = false;
        /// <summary>
        /// 計測終了したかを返す関数
        /// </summary>
        public bool IsEnded()
            => Timer > DurationTime;
    }

    /// <summary>
    /// 現在抱えているタスク
    /// </summary>
    private Dictionary<int, TaskStruct> _myTasks;

    /// <summary>
    /// タスクを管理するための連想配列
    /// </summary>
    private int _id;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Durator()
        => Initialize();

    /// <summary>
    /// 初期化関数
    /// </summary>
    public void Initialize()
    {
        _id = 0;
        _myTasks = new();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    public void Update()
    {
        if (_myTasks.Count < 1)
            return;

        foreach (var key in _myTasks.Keys)
        {
            //計測中か？
            if (_myTasks[key].IsMeasuring)
            {
                //タイマーを進める
                _myTasks[key].CountTimer();

                //関数実行
                ExecutionFunction(key);

                //まだ終了してなければループ続行
                if (!_myTasks[key].IsEnded())
                    continue;
                
                //終了していれば終了処理を呼び出す
                EndTask(key);

                //連想配列の長さが変わるので
                //ループ終了
                break;
            }
        }
    }

    /// <summary>
    /// 依頼された持続処理を実行する関数
    /// </summary>
    public void ExecutionFunction(int key)
    {
        //渡された関数を実行
        _myTasks[key].Func.Invoke(_myTasks[key].Timer, _myTasks[key].DurationTime);
    }

    /// <summary>
    /// 持続時間終了時の処理
    /// </summary>
    /// <param name="key">終了するタスクのキー</param>
    /// <param name="isCallBack">コールバックを実行するか</param>
    public void EndTask(int key, bool isCallBack = true)
    {
        //渡されたコールバックを実行
        if (isCallBack)
            _myTasks[key].CallBack?.Invoke();

        //依頼を達成したので除外する
        _myTasks.Remove(key);
    }

    /// <summary>
    /// 時間経過で実行したい関数を登録して
    /// 実行タスクを生成する関数
    /// </summary>
    /// <param name="func">依頼したいコールバック</param>
    /// <param name="waitTime">待ち時間</param>
    /// <returns>このタスクの識別番号</returns>
    public int CreateTask(DurationTask func, CallBackTask callBack, float waitTime, bool onUnscaledTime = false)
    {
        //生成IDをインクリメント
        ++_id;
        //タスク生成
        TaskStruct newTask = new();
        //持続処理登録
        newTask.Func = func;
        //コールバック登録
        newTask.CallBack = callBack;
        //待ち時間登録
        newTask.DurationTime = waitTime;
        //タイマー初期化
        newTask.Timer = 0;
        //計測フラグを立てる
        newTask.IsMeasuring = true;
        // 計測モードを設定
        newTask.OnUnscaledTime = onUnscaledTime;
        //連想配列に格納
        _myTasks.Add(_id, newTask);
        //登録に使用した識別番号を返却
        return _id;
    }

    /// <summary>
    /// 指定のタスクの計測を再開する
    /// </summary>
    public void ReAwakeTask(int key)
        => _myTasks[key].StartMeasuring();

    /// <summary>
    /// 指定のタスクの計測を中断する
    /// </summary>
    public void StopTask(int key)
        => _myTasks[key].StopMeasuring();

    /// <summary>
    /// 渡されたタスクをキャンセルする関数
    /// </summary>
    public void CanncellTask(int key, bool isCallBack = false)
        => EndTask(key, isCallBack);

    /// <summary>
    /// IDからタスクを取得する処理
    /// </summary>
    /// <param name="key">取得したいタスクのID</param>
    /// <returns>存在すればそのタスクを返し、しなければnullを返す</returns>
    public TaskStruct GetTaskFromID(int key)
    {
        if (!_myTasks.ContainsKey(key))
            return null;

        return _myTasks[key];
    }

    /// <summary>
    /// そのIDのタスクが実行中かを取得する関数
    /// </summary>
    /// <param name="key">取得したいタスクのID</param>
    /// <returns>実行中かどうか</returns>
    public bool IsTaskExistTask(int key)
    {
        var task = GetTaskFromID(key);

        //タスクが存在しなければfalse
        if (task == null)
            return false;

        //計測中かつ、終了してなければ実行中である
        return task.IsMeasuring && !task.IsEnded();
    }
}
