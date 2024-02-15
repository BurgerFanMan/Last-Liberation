using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSpeed : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] float _animationSpeed;

    void Start()
    {
        _animator = _animator == null ? GetComponent<Animator>() : _animator;
    }

    void Update()
    {
        _animator.speed = Pause.adjTimeScale * _animationSpeed;
    }
}
