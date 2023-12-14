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
    public void AssignManager(EnemyManager enemyMan)
    {
        _enemyManager = enemyMan;
    }

    private void Update()
    {
        navAgent.speed = _speed * Pause.timeScale;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Building")
        {
            _enemyManager.DamageBuilding(_damage, collision.gameObject);
            DeathAction();
        }
    }

    protected override void DeathAction()
    {
        _enemyManager.KillEnemy(this);
    }
    public void SetTarget(Vector3 target)
    {
        navAgent.SetDestination(target);
    }
}
