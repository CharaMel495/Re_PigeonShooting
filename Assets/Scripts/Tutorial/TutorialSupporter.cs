using UnityEngine;
using Tutorial;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

/// <summary>
/// TutorialManagerの機能を一部代替わりして持つサポータークラス
/// TutorialManagerに何でもかんでもかかないようにする処置
/// </summary>
public class TutorialSupporter
{
    private TutorialUI _tutorialUI;

    private TutorialTexts _tutorialTexts;

    public TutorialSupporter(TutorialUI ui)
    {
        _tutorialUI = ui;
        _tutorialTexts = Addressables.LoadAssetAsync<TutorialTexts>(SummarizeResourceDirectory.TUTORIALTEXTS).WaitForCompletion();
    }

    public void SetUp(Tutorials tutorial)
    {
        TutorialPlayerManager.Instance.EnActivePlayer(CreateArrowedAction(tutorial));
        _tutorialUI.gameObject.SetActive(true);
        _tutorialUI.Initialize(_tutorialTexts.GetTutorialText(tutorial));
    }

    public bool PlayNext()
    {
        return _tutorialUI.PlayNext();
    }

    private Dictionary<Tutorials, bool> CreateArrowedAction(Tutorials tutorial)
    {
        return tutorial switch
        {
            Tutorials.Move => new Dictionary<Tutorials, bool>
            {
                { Tutorials.Move, true },
                { Tutorials.Shot, false },
                { Tutorials.Vacuum, false },
                { Tutorials.Dash, false },
                { Tutorials.AirBaster, false },
            },
            Tutorials.Shot => new Dictionary<Tutorials, bool>
            {
                { Tutorials.Move, false },
                { Tutorials.Shot, true },
                { Tutorials.Vacuum, false },
                { Tutorials.Dash, false },
                { Tutorials.AirBaster, false },
            },
            Tutorials.Vacuum => new Dictionary<Tutorials, bool>
            {
                { Tutorials.Move, false },
                { Tutorials.Shot, true },
                { Tutorials.Vacuum, true },
                { Tutorials.Dash, false },
                { Tutorials.AirBaster, false },
            },
            Tutorials.Dash => new Dictionary<Tutorials, bool>
            {
                { Tutorials.Move, true },
                { Tutorials.Shot, false },
                { Tutorials.Vacuum, false },
                { Tutorials.Dash, true },
                { Tutorials.AirBaster, false },
            },
            Tutorials.AirBaster => new Dictionary<Tutorials, bool>
            {
                { Tutorials.Move, true },
                { Tutorials.Shot, true },
                { Tutorials.Vacuum, true },
                { Tutorials.Dash, false },
                { Tutorials.AirBaster, true },
            },
            _ => null
        };
    }
}
