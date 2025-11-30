using UnityEngine;

public class ShuBoss : BossBase
{
    private enum AnimState
    {
        Open,
        Close
    }

    private enum ActionState
    {

    }
    private ActionState _state;

    [SerializeField]
    private Animator _animator;

    private int _changeAnimTriggerHash;


    public override void Initialize()
    {
        _changeAnimTriggerHash = Animator.StringToHash("Change");
    }

    public override void Smashed()
    {

    }

    public override void Action()
    {

    }

    public override void OnHit()
    {

    }
}
