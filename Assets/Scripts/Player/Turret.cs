using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Turret : IFireBullets
{
    [Header("Weaponry")]
    [SerializeField] protected float _reloadTime = 5f;
    [SerializeField] protected float _fireRate = 1f;
    [SerializeField] protected int _magazineCapacity = 2;
    [SerializeField] protected float turretRotationSpeed;
    [SerializeField] protected List<Transform> firePoints;
    [SerializeField] protected List<Transform> turretsYAxis;

    [Header("Distance and Location")]
    [SerializeField] protected float _maxRange = 20f;
    [SerializeField] protected float _minRange = 0f;
    [SerializeField] protected bool _calculateLead;
    [SerializeField] protected float _leadRatio = 1f;

    [Header("Debugging")]
    [SerializeField] protected Transform enemyT;

    protected EnemyManager enemyManager;

    private float timeSinceFire;
    private float timeSinceReload;

    private int numbOfFirePoints;
    private int shellsLeft;

    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();

        numbOfFirePoints = firePoints.Count;
    }

    private void Update()
    {
        bool gotTarget = EnemyInRange(out Transform enemy);

        if (gotTarget)
        {
            RotateTurrets(enemy);
        }

        if (shellsLeft == 0)
        {
            timeSinceReload += Pause.adjTimeScale;

            if(timeSinceReload > _reloadTime)
            {
                shellsLeft = _magazineCapacity;

                timeSinceReload = 0f;
            }
            else return;
        }

        if (timeSinceFire < 1f / _fireRate)
        {
            timeSinceFire += Pause.adjTimeScale;

            return;
        }

        if (!gotTarget)
            return;

        timeSinceFire = 0f;
        shellsLeft -= 1;

        FireShell();
    }

    protected virtual void RotateTurrets(Transform enemy)
    {
        Vector3 enemyPosition = enemy.position;

        if (_calculateLead)
            enemyPosition += (enemyT.position - transform.position).magnitude * _leadRatio * enemyT.transform.forward;

        foreach (Transform turret in turretsYAxis)
        {
            Vector3 targetRotEuler = Quaternion.LookRotation(enemyPosition - turret.position).eulerAngles;
            Quaternion targetRot = Quaternion.Euler(turret.rotation.x, targetRotEuler.y + 90f, turret.rotation.z);
            turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, turretRotationSpeed * Pause.adjTimeScale);
        }
    }

    protected virtual void FireShell()
    {
        Transform firePoint = firePoints[Random.Range(0, numbOfFirePoints)];

        SpawnBullet(firePoint.position, firePoint.rotation);
    }

    protected bool EnemyInRange(out Transform closestEnemy)
    {
        closestEnemy = null;
        float dist = _maxRange;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy != null)
            {
                float thisDist = Vector3.Distance(enemy.transform.position, transform.position);
                if (thisDist < dist && thisDist >= _minRange)
                {
                    dist = thisDist;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return (closestEnemy != null);
    } 
}
