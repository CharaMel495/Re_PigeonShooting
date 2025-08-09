using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ScoreRanking", menuName = "Scriptable Objects/ScoreRanking")]
public class ScoreRanking : ScriptableObject
{
    public List<int> ScoreList;

    public void ResetList()
    {
        ScoreList = new();
    }

    public void AddScore()
    {

    }
}
