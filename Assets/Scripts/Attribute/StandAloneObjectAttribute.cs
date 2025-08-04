using UnityEngine;
using System;

/// <summary>
/// マネージャー等がつかない独立したクラスであることを表す
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class StandAloneObjectAttribute : Attribute
{
}
