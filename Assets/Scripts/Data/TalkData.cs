using System.Linq;
using Tutorial;
using UnityEngine;

public enum FaceType
{
    Normal,
    Smile,
    Damaged
}

[System.Serializable]
public class TextSet
{
    public FaceType FaceType;

    [TextArea]
    public string Text;
}

[System.Serializable]
public class FacePair
{
    public FaceType Type;
    public Sprite FaceTexture;
}

public enum TalkType
{
    TitleTexts
}

[CreateAssetMenu(fileName = "TalkData.asset", menuName = "Scriptable Objects/TalkData")]
public class TalkData : ScriptableObject
{
    public TextSet[] TitleTexts;
    public FacePair[] FacePair;

    public TextSet[] GetTalkText(TalkType tutorial)
    {
        return tutorial switch
        {
            TalkType.TitleTexts => TitleTexts,
            _ => null
        };
    }

    public Sprite GetFaceSprite(FaceType type)
    {
        return FacePair.Single((faces) => faces.Type == type).FaceTexture;
    }
}