using System;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField]
    private SpriteRendererWrapper _renderer;

    [SerializeField]
    private Animator _animator;

    private Action _animationCallBack;

    private readonly int _playhash = Animator.StringToHash("Play");

    public void PlayParticle(Action callback = null)
    {
        _renderer.Initialize();

        _renderer.SetEnabled(true);
        _animationCallBack = callback;
        _animator.SetTrigger(_playhash);
    }

    public void OnEndAnimation()
    {
        _animationCallBack?.Invoke();
        _renderer.SetEnabled(false);
    }
}
