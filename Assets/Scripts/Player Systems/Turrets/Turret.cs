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
    public float maxRange = 20f;
    public float minRange = 0f;
    [SerializeField] protected bool _calculateLead;
    [SerializeField] protected float _leadRatio = 1f;
    [SerializeField] protected bool _limitAngle = true;
    public float angleRange = 60f; //range of angle to both clockwise and counterclockwise directions

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
            enemyPosition += (enemy.position - transform.position).magnitude * _leadRatio * enemy.transform.forward;

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

    //Utility functions
    protected bool EnemyInRange(out Transform closestEnemy)
    {
        closestEnemy = null;
        float dist = maxRange;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (enemy != null)
            {
                float thisDist = Vector3.Distance(enemy.transform.position, transform.position);
                if (thisDist < dist && thisDist >= minRange && IsInAngleRange(enemy.transform.position))
                {
                    dist = thisDist;
                    closestEnemy = enemy.transform;
                }
            }
        }

        return (closestEnemy != null);
    }
    protected bool IsInAngleRange(Vector3 position)
    {
        if (!_limitAngle)
            return true;

        float angleDif = Vector3.Angle(position - transform.position, -transform.forward);

        if(angleDif < angleRange)
        {
            return true;
        }

        return false;
    }
    protected float NormalizeAngle(float angle)
    {
        return (angle + 360f) % 360f;
    }
}
