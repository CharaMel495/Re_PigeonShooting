using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerUI : MonoBehaviour
{
    [Header("UIの参照")]
    [SerializeField]
    private SelfMade.Slider _expSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        EventDispatcher.Instance.Bind(this);
        _expSlider.UpdateValue(0.0f);
    }

    [CallableEvent("UpdateEXP")]
    private void UpdateEXP(object data)
    {
        if (data is not float value)
            return;

        _expSlider.UpdateValue(value);
    }
}
