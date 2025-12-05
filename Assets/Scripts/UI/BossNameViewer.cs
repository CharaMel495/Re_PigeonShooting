using UnityEngine;
using DG.Tweening;

public class BossNameViewer : MonoBehaviour
{
    [SerializeField]
    private TextWrapper _text;

    [SerializeField]
    private float _irisOutTime;

    [SerializeField]
    private ImageWrapper _image;

    [SerializeField]
    private Vector3 _irisOutSize;

    [SerializeField]
    private Vector3 _normalSize;

    private Timer _timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        _text.Initialize();
        _image.Initialize();
        _timer = new();
        _timer.Initialize();

        EventDispatcher.Instance.Bind(this);
    }

    private void FixedUpdate()
    {
        _timer.Update();
    }

    [CallableEvent("ViewBossName")]
    public void IrisOut(object data)
    {
        if (data is string name)
        {
            this.gameObject.SetActive(true);

            this.GetComponent<RectTransform>().DOScale(_irisOutSize, _irisOutTime).
                OnComplete(ViewBossName);
        }

        void ViewBossName()
        {
            _text.SetText(name);
        }
    }

    [CallableEvent("StartBossBattle")]
    public void HideBossName(object data)
    {
        _text.SetText("");

        this.transform.DOScale(_normalSize, _irisOutTime).OnComplete(() => this.gameObject.SetActive(false));
    }
}
