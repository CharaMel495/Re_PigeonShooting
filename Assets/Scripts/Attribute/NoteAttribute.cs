using System;
using UnityEngine;

/// <summary>
/// メソッド限定のメモ書き可能にするアトリビュート
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class NoteAttribute : Attribute
{
    public string Note { get; }
    public NoteAttribute(string note)
        => Note = note;
}