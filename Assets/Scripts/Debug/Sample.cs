using UnityEngine;

public class DebugData : MonoBehaviour
{
    [SerializeField]
    private int task = 2525;

    [ReadOnlySerializeField("スコア", 0.0f, 0.0f, 1.0f)] 
    private int score = 42;

    [ReadOnlySerializeField("現在の状態", 0.0f, 1.0f, 0.0f)] 
    public string status = "Ready";

    [ReadOnlySerializeField] 
    private float internalValue = 3.14f;

    private void FixedUpdate()
    {
        score++;
    }
}
