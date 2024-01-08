using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Turret : IFireBullets
{
    [Header("Weaponry")]
    [SerializeField] protected float _reloadTime = 5f;
    [SerializeField] protected float _fireRate = 1f;
    [SerializeField] protected int _magazineCapacity = 2;
    [SerializeField] protected float _turretRotationSpeed;
    public List<SubTurret> subTurrets;

    [Header("Distance and Location")]
    public float maxRange = 20f;
    public float minRange = 0f;
    [SerializeField] protected bool _calculateLead;
    [SerializeField] protected float _leadRatio;
    [SerializeField] protected bool _limitAngle = true;
    public float angleRange = 60f; //range of angle to both clockwise and counterclockwise directions
    [SerializeField] protected bool _waitTillAimingAtEnemy; //if true, the turret will not fire till it is directly facing the enemy to avoid wasting shells while retargeting

    [Header("Debugging")]
    [SerializeField] protected Transform enemyT;

    public List<SubTurretClass> subTurretClasses = new List<SubTurretClass>();

    protected EnemyManager enemyManager;

    private float timeSinceFire;
    private float timeSinceReload;

    private int shellsLeft;

    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
    }

    private void Start()
    {   //takes the values from the subTurret structs and inputs them in a new class
        for(int i = 0; i < subTurrets.Count; i++)
        {
            SubTurret subTurret = subTurrets[i];

            SubTurretClass subTurretClass = new SubTurretClass
            {
                startingForward = -subTurret.turretBase.right,
                turretBase = subTurret.turretBase,
                firePoint = subTurret.firePoint
            };

            subTurretClasses.Add(subTurretClass);
        }
    }

    private void Update()
    {   //Checks if there is a valid target for each of the subturrets, then rotates them to face that target
        for(int i = 0; i < subTurretClasses.Count; i++)
        {
            SubTurretClass subTurret = subTurretClasses[i];

            bool gotTarget = EnemyInRange(subTurret, out Transform enemy);

            if (gotTarget)
            {
                Vector3 enemyPosition = enemy.position;
                if (_calculateLead)
                    enemyPosition += (enemy.position - transform.position).magnitude * _leadRatio * enemy.transform.forward;

                RotateTurret(subTurret, enemyPosition);
            }
            else
                subTurret.aimingAtEnemy = false;
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

        if (_waitTillAimingAtEnemy)
        {   //adds all subturrets that are aiming at the enemy to a list
            List<SubTurretClass> shootableSubTurrets = new List<SubTurretClass>();

            shootableSubTurrets.AddRange(subTurretClasses.Where(subTurret => subTurret.aimingAtEnemy == true));

            if (shootableSubTurrets.Count == 0)
                return;

            FireShell(shootableSubTurrets);
        }
        else
            FireShell(subTurretClasses);
    }

    protected virtual void RotateTurret(SubTurretClass subTurret, Vector3 enemyPosition)
    {
        Transform turret = subTurret.turretBase;

        Vector3 targetRotEuler = Quaternion.LookRotation(enemyPosition - turret.position).eulerAngles;
        Quaternion targetRot = Quaternion.Euler(turret.rotation.x, targetRotEuler.y + 90f, turret.rotation.z);

        turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, _turretRotationSpeed * Pause.adjTimeScale);

        subTurret.aimingAtEnemy = GetAngleDifference(targetRotEuler.y + 90f, turret.eulerAngles.y) < 5f;
    }
    protected virtual void FireShell(List<SubTurretClass> subTurretsToFire)
    {
        timeSinceFire = 0f;
        shellsLeft -= 1;
        
        Transform firePoint = subTurretsToFire[Random.Range(0, subTurretsToFire.Count)].firePoint;

        SpawnBullet(firePoint.position, firePoint.rotation);
    }

    //Utility functions
    protected bool EnemyInRange(SubTurretClass subTurret, out Transform closestEnemy)
    {
        closestEnemy = null;
        float dist = maxRange;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            float thisDist = Vector3.Distance(enemy.transform.position, subTurret.turretBase.position);
            if (thisDist < dist && thisDist >= minRange && IsInAngleRange(enemy.transform.position))
            {
                dist = thisDist;
                closestEnemy = enemy.transform;
            }          
        }

        return (closestEnemy != null);

        bool IsInAngleRange(Vector3 position)
        {
            if (!_limitAngle)
                return true;

            float angleDif = Vector3.Angle(position - subTurret.turretBase.position, subTurret.startingForward);
            
            if (angleDif < angleRange)
            {
                return true;
            }

            return false;
        }
    }
    private float GetAngleDifference(float angle1, float angle2)
    {
        float diff = (angle2 - angle1) % 360f;
        return diff < 0f ? diff + 360f : diff;
    }
}

[System.Serializable]
public struct SubTurret
{
    public Transform firePoint;
    public Transform turretBase;
}

public class SubTurretClass
{
    public Transform firePoint;
    public Transform turretBase;

    public Vector3 startingForward;

    public bool aimingAtEnemy;
}