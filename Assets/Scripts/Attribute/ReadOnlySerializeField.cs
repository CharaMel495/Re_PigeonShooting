using UnityEngine;
using System;

/// <summary>
/// エディタ上で編集不可なSerializeField
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlySerializeFieldAttribute : Attribute
{
    public string Note { get; }
    public Color NoteColor { get; }
    public ReadOnlySerializeFieldAttribute(string note = "", float r = 1f, float g = 1f, float b = 1f)
    {
        Note = note;
        NoteColor = new Color(r, g, b);
    }
}
