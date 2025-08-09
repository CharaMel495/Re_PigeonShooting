using UnityEngine;

[CreateAssetMenu(fileName = "ScoreHolder", menuName = "Scriptable Objects/ScoreHolder")]
public class ScoreHolder : ScriptableObject
{
    [ReadOnlySerializeField]
    public int Score;

    public void Initialize()
        => Score = 0;
}
