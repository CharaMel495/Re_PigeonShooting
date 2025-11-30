using UnityEngine;

public class ShuBoss : MonoBehaviour
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

    public void Initialize()
    {
        _changeAnimTriggerHash = Animator.StringToHash("Change");
    }

    private void Start()
        => Initialize();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            _animator.SetTrigger(_changeAnimTriggerHash);
    }
}
