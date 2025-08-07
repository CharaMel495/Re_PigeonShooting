using UnityEngine;
using Tutorial;
using System.Collections.Generic;

namespace Tutorial
{
    public enum CheckLists
    {
        None,
        Move,
        Move_ItemGet,
        Shot,
        Shot_SmashEnemy,
        Vacuum,
        Vacuum_Item,
        Dash,
        Dash_Invincible,
        AirBaster_GetDust,
        AirBaster_Shot,
        AirBaster
    }
}

/// <summary>
/// チュートリアルの完了報告が来た時に、現在のチュートリアルかを調べてチェックを進めるクラス
/// </summary>
public class TutorialChecker : MonoBehaviour
{
    private Dictionary<Tutorials, CheckLists[]> _checkDic;

    private int _checkIdx;

    private CheckLists[] _currentChecking;

    [SerializeField]
    private CheckListUI[] _checkList;

    private CheckListUI _currentUI;

    private bool _isChecking = false;

    public void Initialize()
    {
        EventDispatcher.Instance.Bind(this);

        _checkDic = new Dictionary<Tutorials, CheckLists[]>
        {
            { 
                Tutorials.Move, new CheckLists[]
                {
                    CheckLists.Move,
                    CheckLists.Move_ItemGet
                }
            },
            {
                Tutorials.Shot, new CheckLists[]
                {
                    CheckLists.Shot,
                    CheckLists.Shot_SmashEnemy
                }
            },
            {
                Tutorials.Vacuum, new CheckLists[]
                {
                    CheckLists.Vacuum,
                    CheckLists.Vacuum_Item
                }
            },
            {
                Tutorials.Dash, new CheckLists[]
                {
                    CheckLists.Dash,
                    CheckLists.Dash_Invincible
                }
            },
            {
                Tutorials.AirBaster, new CheckLists[]
                {
                    CheckLists.AirBaster_GetDust,
                    CheckLists.AirBaster_Shot,
                    CheckLists.AirBaster,
                }
            },
        };
    }

    public void SetUp(Tutorials tutorial)
    {
        foreach (var item in _checkList)
            item.Initialize();

        _isChecking = false;
        _checkIdx = 0;
        _currentChecking = _checkDic[tutorial];
        _currentUI = _checkList[(int)tutorial];
        _currentUI.EnActive();
    }

    public CheckLists EnterCheckingMode()
    {
        if (_checkIdx >= _currentChecking.Length)
            return CheckLists.None;

        // プレイヤーの操作が効くように
        InputManager.Instance.ChangeInputHandler(InputHandler.Player);
        _isChecking = true;

        return _currentChecking[_checkIdx];
    }

    public void ExitCheckingMode()
    {
        // UIの操作に戻す
        InputManager.Instance.ChangeInputHandler(InputHandler.UI);
        _isChecking = false;
        ++_checkIdx;
    }

    [CallableEvent("CheckTutorial")]
    public void CheckTutorial(object data)
    {
        if (!_isChecking)
            return;

        CheckLists recieveData = (CheckLists)data;

        if (recieveData != _currentChecking[_checkIdx])
            return;

        // チュートリアル成功イベントを呼び出す
        EventDispatcher.Instance.Dispatch("CorrectTutorial");

        _currentUI.WhenCorrect();

        // チュートリアルの遂行を見張るのを停止
        ExitCheckingMode();
    }
}
