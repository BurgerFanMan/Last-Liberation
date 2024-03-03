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
    [SerializeField] float _timeToDetonate = 2f;
    [SerializeField] float _startingTimeToFlash = 0.5f;
    [SerializeField] GameObject _detonationEffect;
    [SerializeField] MeshRenderer _explosiveRenderer;
    [SerializeField] Color _flashColor = Color.white;
 
    private EnemyManager _enemyManager;

    private bool detonating;
    private float timeDone;

    //flashing explosive device stuff
    private float flashTimeDone;
    private float timeLeftTillFlash;
    private float timeToFlash;

    private bool isFlashing;
    private Color oldColor;

    private GameObject target;

    private void Start()
    {
        navAgent.speed = _speed;
        timeToFlash = _startingTimeToFlash;

        oldColor = _explosiveRenderer.material.GetColor("_EmissionColor");
    }

    private void Update()
    {
        if (target != null && detonating)
        {
            navAgent.speed = 0f;

            if(timeDone <= _timeToDetonate)
            {
                timeDone += Pause.adjTimeScale;

                if(timeLeftTillFlash < timeToFlash)
                {
                    timeLeftTillFlash += Pause.adjTimeScale;

                    return;
                }

                if (!isFlashing)
                    FlashStart();

                if(flashTimeDone < 0.1f)
                {
                    flashTimeDone += Pause.adjTimeScale;

                    return;
                }

                FlashEnd();

                return;
            }

            Detonate();
        }

        navAgent.speed = _speed * Pause.timeScale;


        void FlashStart()
        {
            isFlashing = true;

            if (GetComponent<AudioSource>() != null)
                GetComponent<AudioSource>().Play();

            _explosiveRenderer.material.SetColor("_EmissionColor", _flashColor);
        }
        void FlashEnd()
        {
            isFlashing = false;

            timeLeftTillFlash = 0f;
            flashTimeDone = 0f;
            timeToFlash /= 2f;

            _explosiveRenderer.material.SetColor("_EmissionColor", oldColor);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Building")
        {
            target = collision.gameObject;

            BeginDetonating();
        }
    }

    public void SetTarget(Vector3 target)
    {
        navAgent.SetDestination(target);
    }
    public void AssignManager(EnemyManager enemyMan)
    {
        _enemyManager = enemyMan;
    }

    protected override void DeathAction()
    {
        _enemyManager.KillEnemy(this);
    }
    
    void BeginDetonating()
    {
        detonating = true;

        timeDone = 0f;
        flashTimeDone = 0f;
        timeLeftTillFlash = 0f;

        timeToFlash = _startingTimeToFlash;
    }
    void Detonate()
    {
        if(target != null)
            _enemyManager.DamageBuilding(_damage, target);

        if (_detonationEffect != null)
        {
            Instantiate(_detonationEffect, transform.position, transform.rotation);  
        }

        DeathAction();
    }
}
