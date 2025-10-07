using UnityEngine;

public class DashBatteryUI : MonoBehaviour
{
    [SerializeField]
    private ImageWrapper[] _batteryes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {
        foreach (var img in _batteryes)
            img.Initialize();
    }

    public void OpenBattery(int num)
    {
        _batteryes[num].SetImageColor(Color.white);
    }

    public void CloseBattery(int num)
    {
        _batteryes[num].SetImageColor(Color.black);
    }
}
