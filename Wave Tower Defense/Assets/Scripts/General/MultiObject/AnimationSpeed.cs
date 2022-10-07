using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeed : ICanBePaused
{
    [SerializeField] Animator _animator;
    [SerializeField] bool _usePausedSpeed;

    [System.NonSerialized] public float _animationSpeed;
    void Start()
    {
        _animator = _animator == null ? GetComponent<Animator>() : _animator;
    }

    void Update()
    {
        float animspeed = _usePausedSpeed ? _timeScale : _animationSpeed;
        _animator.speed = animspeed;
    }
}
