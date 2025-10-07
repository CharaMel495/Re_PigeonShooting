using UnityEngine;

public class EXPUI : MonoBehaviour
{
    [SerializeField]
    private SelfMade.Slider _expSlider;

    [SerializeField]
    private ImageWrapper _forecastIcon;

    public void Initialize()
    {
        _expSlider.UpdateValue(0.0f);
        _forecastIcon.Initialize();
        EventDispatcher.Instance.Bind(this);
    }

    [CallableEvent("EXPUIInitialSet")]
    public void InitialForeCastSet(object data)
    {
        if (data is not PlayerGrowStatus.GrowStatus grow)
            return;

        _forecastIcon.SetSprite(GetForeCastIcon(grow));
    }

    [CallableEvent("UpdateEXP")]
    public void UpdateEXP(object data)
    {
        if (data is not float value)
            return;

        _expSlider.UpdateValue(value);
    }

    [CallableEvent("SetNextGrowIcon")]
    public void WhenLevelUp(object data)
    {
        if (data is not PlayerGrowStatus.GrowStatus grow)
            return;

        _forecastIcon.SetSprite(GetForeCastIcon(grow));
    }

    private Sprite GetForeCastIcon(PlayerGrowStatus.GrowStatus grow)
    {
        return grow switch
        {
            PlayerGrowStatus.GrowStatus.ShotValue => SpriteManager.GetSprite(SpriteData.SpriteType.BulletUPIcon),
            PlayerGrowStatus.GrowStatus.ShotRate => SpriteManager.GetSprite(SpriteData.SpriteType.BulletRateUPIcon),
            PlayerGrowStatus.GrowStatus.ShotPower => SpriteManager.GetSprite(SpriteData.SpriteType.BulletPowerUPIcon),
            PlayerGrowStatus.GrowStatus.ShotProbabirity => SpriteManager.GetSprite(SpriteData.SpriteType.BulletDustProofUPIcon),
            _ => null
        };
    }
}
