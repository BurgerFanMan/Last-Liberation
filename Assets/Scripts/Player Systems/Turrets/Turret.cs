using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class Turret : IFireBullets
{
    [Header("Weaponry")]
    public float reloadTime = 5f;
    public float fireRate = 1f;
    public int magazineCapacity = 2;
    [SerializeField] protected float _turretRotationSpeed;
    public List<SubTurret> subTurrets;

    [Header("Targeting")]
    public float maxRange = 20f;
    [SerializeField] protected bool _calculateLead;
    [SerializeField] protected float _leadRatio;
    [SerializeField] protected bool _limitAngle = true;
    public float angleRange = 60f; //range of angle to both clockwise and counterclockwise directions
    [SerializeField] protected bool _waitTillAimingAtEnemy = true; //if true, the turret will not fire till it is directly facing the enemy to avoid wasting shells while retargeting
    [SerializeField] protected float _angularDistanceToFire = 1f; //the range of angle at which the turret will open fire if wait till aiming is TRUE
    [Range(0f, 1f)]
    [SerializeField] protected float _angularDistanceBias = 0.5f; //how much should the angle to the enemy be considered when finding the closest enemy

    [Header("UI")]
    public GameObject overlayIcon;
    [SerializeField] protected bool overrideSoundOnShoot = true;

    [Header("Debugging")]
    [SerializeField] protected Transform enemyT;

    public List<SubTurretClass> subTurretClasses = new List<SubTurretClass>();

    protected EnemyManager enemyManager;

    private AudioSource audioSource;

    private float timeSinceFire;
    private float timeSinceReload;

    private int shellsLeft;

    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();

        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {   //takes the values from the subTurret structs and inputs them in a new class
        for(int i = 0; i < subTurrets.Count; i++)
        {
            SubTurret subTurret = subTurrets[i];

            SubTurretClass subTurretClass = new SubTurretClass
            {
                startingAngle = subTurret.turretBase.localEulerAngles.y,
                startingForward = -subTurret.turretBase.right,
                turretBase = subTurret.turretBase,
                firePoints = subTurret.firePoints
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

                subTurret.targetPosition = enemyPosition;
            }
            else
                subTurret.aimingAtEnemy = false;
        }     

        if (shellsLeft == 0)
        {
            timeSinceReload += Pause.adjTimeScale;

            if(timeSinceReload > reloadTime)
            {
                shellsLeft = magazineCapacity;

                timeSinceReload = 0f;
            }
            else return;
        }

        if (timeSinceFire < 1f / fireRate)
        {
            timeSinceFire += Pause.adjTimeScale;

            return;
        }


        List<SubTurretClass> shootableSubTurrets = new List<SubTurretClass>();

        shootableSubTurrets.AddRange(subTurretClasses.Where(subTurret => subTurret.aimingAtEnemy == true));

        if (shootableSubTurrets.Count == 0)
            return;

        FireShell(shootableSubTurrets);
    }

    protected virtual void RotateTurret(SubTurretClass subTurret, Vector3 enemyPosition)
    {
        Transform turret = subTurret.turretBase;

        Vector3 targetRotEuler = Quaternion.LookRotation(enemyPosition - turret.position).eulerAngles;
        Quaternion targetRot = Quaternion.Euler(turret.rotation.x, targetRotEuler.y + 90f, turret.rotation.z);

        turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRot, _turretRotationSpeed * Pause.adjTimeScale);

        subTurret.aimingAtEnemy = !_waitTillAimingAtEnemy ? true :
            GetAngleDifference(targetRotEuler.y + 90f, turret.eulerAngles.y) < _angularDistanceToFire;
    }
    protected virtual void FireShell(List<SubTurretClass> subTurretsToFire)
    {
        timeSinceFire = 0f;
        shellsLeft -= 1;

        SubTurretClass subTurret = subTurretsToFire[Random.Range(0, subTurretsToFire.Count)];
        Transform firePoint = subTurret.firePoints[Random.Range(0, subTurret.firePoints.Count)];

        SpawnBullet(firePoint.position, firePoint.rotation, subTurret.targetPosition);

        if (audioSource != null && !(!overrideSoundOnShoot && audioSource.isPlaying))
            audioSource.Play();
    }

    //Utility functions
    protected bool EnemyInRange(SubTurretClass subTurret, out Transform closestEnemy)
    {
        closestEnemy = null;
        float lowestScore = Mathf.Infinity;
        foreach (Enemy enemy in enemyManager.enemies)
        {
            if (!IsInAngleRange(enemy.transform.position))
                continue;

            float score = 0f;
            float distanceScore = (enemy.transform.position - subTurret.turretBase.position).sqrMagnitude;

            if (distanceScore > maxRange * maxRange)
                continue;

            float angleScore = Vector3.Angle(enemy.transform.position - subTurret.turretBase.position, -subTurret.turretBase.right) / 8f;
            angleScore *= angleScore;

            score += distanceScore * 1f - _angularDistanceBias;
            score += angleScore * _angularDistanceBias;

            if (lowestScore < score)
                continue;

            lowestScore = score;
            closestEnemy = enemy.transform;
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
    public Transform turretBase;
    public List<Transform> firePoints;
}

public class SubTurretClass
{
    public float startingAngle;

    public bool aimingAtEnemy;

    public Vector3 startingForward;
    public Vector3 targetPosition; 

    public Transform turretBase;
    public List<Transform> firePoints;
}