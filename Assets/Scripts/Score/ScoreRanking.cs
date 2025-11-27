using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScoreRanking", menuName = "Scriptable Objects/ScoreRanking")]
public class ScoreRanking : ScriptableObject
{
    public List<int> ScoreList;

    public void ResetList()
    {
        for (int i = 0; i < ScoreList.Count; ++i)
            ScoreList[i] = 0;
    }

    public int GetHighScore()
        => ScoreList.First();

    public void AddScore(ScoreHolder newScore)
    {
        ScoreList.Add(newScore.Score);

        ScoreList = ScoreList
            .OrderByDescending(s => s) // ← ここで降順！
            .Take(30)
            .ToList();
    }
}
