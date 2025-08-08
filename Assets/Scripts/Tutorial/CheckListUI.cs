using UnityEngine;
using DG.Tweening;

/// <summary>
/// チュートリアル用のチェックリストUIを操作するクラス
/// </summary>
public class CheckListUI : MonoBehaviour
{
    [SerializeField]
    private ImageWrapper[] _checkUI;

    [SerializeField]
    private GameObject[] _uiObj;

    [SerializeField]
    private float _checkTime;

    [SerializeField]
    private Vector3 _checkSize;

    private int _next;

    public void Initialize()
    {
        foreach (var ui in _checkUI)
        {
            ui.Initialize();
            ui.transform.localScale = Vector3.zero;
        }

        foreach (var ui in _uiObj)
            ui.SetActive(false);

        _uiObj[0].SetActive(true);

        _next = 0;

        this.gameObject.SetActive(false);
    }

    public void EnActive()
    {
        Initialize();
        this.gameObject.SetActive(true);
    }

    public void DisActive()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// チュートリアル成功時に呼ぶメソッド
    /// </summary>
    public void WhenCorrect()
    {
        // もし、これ以上リストがなければ
        if (_next >= _checkUI.Length)
            return;

        // チェックマークを出す
        _checkUI[_next].transform.DOScale(_checkSize, _checkTime).SetEase(Ease.OutBounce);

        CRISoundManager.Instance.PlaySE(SFX.TutorialSuccess);

        ++_next;

        // 次のチェックリストを見せる
        if (_next < _uiObj.Length)
            _uiObj[_next].SetActive(true);
    }
}
