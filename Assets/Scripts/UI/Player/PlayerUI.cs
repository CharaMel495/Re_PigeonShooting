using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [Header("UIの参照")]
    [SerializeField]
    private SelfMade.Slider _expSlider;

    [SerializeField]
    private CleanerUI _dustMater;

    [SerializeField]
    private GameObject _scoreUI;

    [SerializeField]
    private TextWrapper _timerUI;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        EventDispatcher.Instance.Bind(this);
        _expSlider.UpdateValue(0.0f);
        _dustMater.gameObject.SetActive(false);
        this.gameObject.SetActive(true);
        _timerUI.Initialize();
        _timerUI.SetTextAlpha(1.0f);
        _scoreUI.SetActive(true);
    }

    [CallableEvent("UpdateEXP")]
    public void UpdateEXP(object data)
    {
        if (data is not float value)
            return;

        _expSlider.UpdateValue(value);
    }

    [CallableEvent("StartBossBattle")]
    public void WhenBossBattleStarted(object data)
    {
        _dustMater.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
        _timerUI.SetTextAlpha(0.0f);
        _scoreUI.SetActive(false);
    }
}
