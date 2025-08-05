using UnityEngine;

public class CleanerUI : MonoBehaviour
{
    [SerializeField]
    private SelfMade.Slider _slider;

    [SerializeField]
    private ImageWrapper _image;

    public void UpdataValue(float value)
    {
        _image.Initialize();

        _slider.UpdateValue(value);

        _image.SetImageAlpha(value < 1.0f ? 0.0f : 1.0f);
    }
}
