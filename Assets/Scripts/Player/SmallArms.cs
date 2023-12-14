using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallArms : IFireBullets
{
    [SerializeField] float _reloadTime = 5f;

    [Header("Distance and Location")]
    [SerializeField] float _maxDistance;
    [SerializeField] float _minDistance;
    [SerializeField] float _fireOffset;
    [SerializeField] float _leadAmount;
    [SerializeField] bool _waitForUpgrade;

    protected EnemyManager enemyManager;
    protected Dictionary<Vector3, float> fireFromPoints = new Dictionary<Vector3, float>();

    private void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
    }
    private void Update()
    {
        List<Enemy> enemies = enemyManager.enemies;

        List<Vector3> increaseCounters = new List<Vector3>();
        List<Vector3> resetCounters = new List<Vector3>();

        if (!_waitForUpgrade || _upgradedTimes >= 1)
        {
            foreach (Vector3 ffp in fireFromPoints.Keys)
            {
                fireFromPoints.TryGetValue(ffp, out float rt);
                if (rt >= _reloadTime && enemies.Count > 0)
                {
                    Transform closest = GetNearestObject(enemies, ffp);
                    if (closest != null)
                    {
                        Vector3 firePos = closest.position + (closest.forward * _leadAmount);
                        float dist = Vector3.Distance(ffp, firePos);
                        if (dist <= _maxDistance * _upgradeLevel[1] && dist >= _minDistance)
                        {
                            Fire(ffp, firePos);
                            resetCounters.Add(ffp);
                        }
                    }
                }
                else
                {
                    increaseCounters.Add(ffp);
                }
            }
            foreach (Vector3 newValue in increaseCounters)
            {
                fireFromPoints[newValue] += Pause.adjTimeScale * _upgradeLevel[0];
            }
            foreach (Vector3 newValue in resetCounters)
            {
                fireFromPoints[newValue] = 0f;
            }

            if (upgradeBullets)
                foreach (Bullet bullet in allBullets)
                {
                    bullet.Upgrade(_upgradeLevel[bulletUpgradeIndex], 0);
                    bullet.Upgrade(_upgradeLevel[bulletUpgradeIndex + 1], 1);
                }
        }
    
    }

    Transform GetNearestObject(List<Enemy> enemies, Vector3 currentPos)
    {
        Transform closest = null;
        float dist = float.MaxValue;
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                float thisDist = Vector3.Distance(enemy.transform.position, currentPos);
                if (thisDist < dist)
                {
                    dist = thisDist;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }
    void Fire(Vector3 from, Vector3 to)
    {
        Quaternion _lookRot = Quaternion.LookRotation(to - from);
        SpawnBullet(from + (_lookRot.eulerAngles.normalized * _fireOffset), _lookRot);

    }

    public void ChangeValues(Vector3 ffp)
    {
        fireFromPoints.Add(ffp, 0f);
    }
    public void ResetValue()
    {
        fireFromPoints = new Dictionary<Vector3, float>();
    }
}
