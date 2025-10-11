using UnityEngine;

public class TutorialPlayerVacuumer : ItemVacuumer
{
    private float _tutorialTime = 2.5f;
    private float _timer = 0.0f;

    public override void Initialize(string vacuumeOwnerName, Transform ownerTransform)
    {
        base.Initialize(vacuumeOwnerName, ownerTransform);
        _timer = 0.0f;
    }

    private void FixedUpdate()
    {
        var euler = this.transform.eulerAngles;
        euler.z -= Time.fixedDeltaTime * 1080.0f;
        this.transform.eulerAngles = euler;

        if (_isActive)
            _timer += Time.fixedDeltaTime;

        if (_timer > _tutorialTime)
        {
            EventDispatcher.Instance.Dispatch("CheckTutorial", Tutorial.CheckLists.Vacuum);
            DisActive();
            _timer = 0.0f;
        }
    }
}
