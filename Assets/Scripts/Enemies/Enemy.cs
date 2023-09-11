using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : ICanTakeDamage
{
    [Header("Local")]
    public NavMeshAgent navAgent;
    [SerializeField] float _damage;
    [SerializeField] float _speed;

    EnemyManager _enemyManager;
    private void Start()
    {
        navAgent.speed = _speed;
    }
    public void Move(Vector3 target)
    {
        navAgent.SetDestination(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Building")
        {
            _enemyManager.DamageBuilding(_damage, collision.gameObject);
            DeathAction();
        }
    }

    public void AssignManager(EnemyManager enemyMan)
    {
        _enemyManager = enemyMan;
    }

    protected override void DeathAction()
    {
        _enemyManager.KillEnemy(this);
    }

    protected override void UpdateTime()
    {
        navAgent.speed = _speed * _timeScale;
    }
}
