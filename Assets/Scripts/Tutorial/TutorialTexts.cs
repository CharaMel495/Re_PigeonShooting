using UnityEngine;
using Tutorial;
using System.Collections.Generic;

namespace Tutorial
{
    public enum FaceType
    {
        Normal,
        Smile,
        Damaged
    }
}

[System.Serializable]
public class TutorialTextSet
{
    public FaceType FaceType;

    [TextArea]
    public string Text;
}

[CreateAssetMenu(fileName = "TutorialTexts.asset", menuName = "Scriptable Objects/TutorialTexts")]
public class TutorialTexts : ScriptableObject
{
    public TutorialTextSet[] MoveTutorial;
    public TutorialTextSet[] ShotTutorial;
    public TutorialTextSet[] VacuumTutorial;
    public TutorialTextSet[] DashTutorial;
    public TutorialTextSet[] AirBasterTutorial;

    public TutorialTextSet[] GetTutorialText(Tutorials tutorial)
    {
        return tutorial switch
        {
            Tutorials.Move => MoveTutorial,
            Tutorials.Shot => ShotTutorial,
            Tutorials.Vacuum => VacuumTutorial,
            Tutorials.Dash => DashTutorial,
            Tutorials.AirBaster => AirBasterTutorial,
            _ => null
        };
    }
}
