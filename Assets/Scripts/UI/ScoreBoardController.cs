using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardController : MonoBehaviour
{
    [SerializeField]
    private ScoreRanking _rankingData;

    [SerializeField]
    private ScoreBoardItem _boardItemPrefab;

    private List<ScoreBoardItem> _boardItems;

    [SerializeField]
    private Transform _boardItemRoot;

    [SerializeField]
    private ScrollRect _scrollRect;
    

    [SerializeField]
    private float _scrollSpeed;

    public void Initialize()
    {
        _boardItems = new();

        for (int i = 0; i < _rankingData.ScoreList.Count; ++i)
        {
            _boardItems.Add(Instantiate(_boardItemPrefab, _boardItemRoot));
        }
    }

    public void EnActive()
    {
        int rank = 0;

        foreach (var score in _rankingData.ScoreList)
        {
            _boardItems[rank].Initialize(rank, score);
            ++rank;
        }

        this.gameObject.SetActive(true);
        _scrollRect.verticalScrollbar.value = 1.0f;
    }

    public void DisActive()
    {
        this.gameObject.SetActive(false);
    }

    public void Scroll(bool isDown)
    {
        var vInput = _scrollSpeed * Time.fixedDeltaTime * (isDown ? -1 : 1);

        if (Mathf.Abs(vInput) > 0.1f)
        {
            _scrollRect.verticalNormalizedPosition += vInput * _scrollSpeed * Time.deltaTime;
            _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
        }
    }
}
