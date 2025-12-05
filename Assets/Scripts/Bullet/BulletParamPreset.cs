using BulletStructs;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletParamPreset", menuName = "Scriptable Objects/BulletParamPreset")]
public class BulletParamPreset : ScriptableObject
{
    public StaraightShoot[] StraightShoot;
    public TwoWayStraightShoot[] TwoWayStraightShoot;
    public ThreeWayShoot[] ThreeWayShoot;
    public FourWayShoot[] FourWayShoot;
    public MultiWayShot[] MultiWayShot;
    public SpreadFourShoot[] SpreadFourShoot;
    public SpreadEightShoot[] SpreadEightShoot;
    public StraightAimingShoot[] StraightAimingShoot;
    public SpreadEightAimingShoot[] SpreadEightAimingShoot;
    public RingShot[] RingShot;
    public FiveWayAndBackMonoShoot[] FiveWayAndBackMonoShoot;
    public LazerParam[] LazerParam;
    public CrossLazerParam[] CrossLazer;
    public CurveAndStraight[] CurveAndStraight;
    public StraightAndCurve[] StraightAndCurve;
    public Curve[] Curve;
    public MultiCurve[] MultiCurve;
    public Bounce[] Bounce;
    public MultiBounce[] MultiBounce;
    public BounceRing[] BounceRing;
}