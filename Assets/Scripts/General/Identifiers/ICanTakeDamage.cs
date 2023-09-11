using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ICanTakeDamage : ICanBePaused
{
    [Header("ICanTakeDamage")]
    [SerializeField] public float _health;
    [SerializeField] public float _maxHealth;
    public UnityEvent _onDamage = new UnityEvent();

    protected new void Awake()
    {
        _health = _maxHealth;
    }

    public virtual void DealDamage(float damage)
    {
        _health -= damage;
        if(_onDamage != null)
            _onDamage.Invoke();

        if (_health <= 0)
            DeathAction();
    }

    protected abstract void DeathAction();
}
