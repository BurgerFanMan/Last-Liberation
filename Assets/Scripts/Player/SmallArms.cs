using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallArms : IFireBullets
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] float _reloadTime = 5f;

    [Header("Distance and Location")]
    [SerializeField] float _maxDistance;
    [SerializeField] float _minDistance;
    [SerializeField] float _fireOffset;
    [SerializeField] float _leadAmount;
    [SerializeField] bool _waitForUpgrade;

    [Header("Debug")]
    [SerializeField] bool _debug;
    [SerializeField] protected List<Transform> _debugObjects;

    protected EnemyManager enemyManager;

    protected List<GameObject> enemies = new List<GameObject>();
    

    protected Dictionary<Vector3, float> fireFromPoints = new Dictionary<Vector3, float>();

    protected override void Awake()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        enemies = enemyToGameObject(enemyManager._enemies);
    }
    private void Update()
    {
        List<Vector3> increaseCounters = new List<Vector3>();
        List<Vector3> resetCounters = new List<Vector3>();

        int i = 0;
        if (!_waitForUpgrade || upgradedTimes >= 1)
        {
            foreach (Vector3 ffp in fireFromPoints.Keys)
            {
                if (_debug)
                {
                    _debugObjects[i].position = ffp;
                    i++;
                }

                fireFromPoints.TryGetValue(ffp, out float rt);
                if (rt >= _reloadTime && enemies.Count > 0)
                {
                    Transform closest = GetNearestObject(enemies, ffp);
                    if (closest != null)
                    {
                        Vector3 firePos = closest.position + (closest.forward * _leadAmount);
                        float dist = Vector3.Distance(ffp, firePos);
                        if (dist <= _maxDistance * upgradeLevel[1] && dist >= _minDistance)
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
                fireFromPoints[newValue] += Time.deltaTime * _timeScale * upgradeLevel[0];
            }
            foreach (Vector3 newValue in resetCounters)
            {
                fireFromPoints[newValue] = 0f;
            }

            if (upgradeBullets)
                foreach (Bullet bullet in bullets)
                {
                    bullet.Upgrade(upgradeLevel[bulletUpgradeIndex], 0);
                    bullet.Upgrade(upgradeLevel[bulletUpgradeIndex + 1], 1);
                }
        }
    
    }
    private void LateUpdate()
    {
        enemies = enemyToGameObject(enemyManager._enemies);
    }

    Transform GetNearestObject(List<GameObject> gos, Vector3 currentPos)
    {
        Transform closest = null;
        float dist = 1000000f;
        foreach (GameObject go in gos)
        {
            if (go != null)
            {
                float thisDist = Vector3.Distance(go.transform.position, currentPos);
                if (thisDist < dist)
                {
                    dist = thisDist;
                    closest = go.transform;
                }
            }
        }
        return closest;
    }
    void Fire(Vector3 from, Vector3 to)
    {
        if (_bulletPrefab != null)
        {
            Quaternion _lookRot = Quaternion.LookRotation(to - from);
            Bullet bullet = Instantiate(_bulletPrefab, from, _lookRot).GetComponent<Bullet>();
            bullets.Add(bullet);
            bullet.transform.position += bullet.transform.forward * _fireOffset;        
            bullet._fireBullets = this;
            bullet.ChangeTime(_timeScale);      
        }
    }


    public void ChangeValues(List<GameObject> enems)
    {
        enemies = enems;
    }
    public void ChangeValues(List<Vector3> ffps)
    {
        foreach (Vector3 ffp in ffps)
        {
            fireFromPoints.Add(ffp, 0f);
        }
    }
    public void ChangeValues(Vector3 ffp)
    {
        fireFromPoints.Add(ffp, 0f);
    }
    public void ResetValue()
    {
        fireFromPoints = new Dictionary<Vector3, float>();
    }
    private List<GameObject> arrayToList(GameObject[] array)
    {
        List<GameObject> b = new List<GameObject>();
        for (int i = array.Length; i > 0; i--)
        {
            b.Add(array[i - 1]);
        }
        return b;
    }
    private List<GameObject> enemyToGameObject(List<Enemy> enem)
    {
        List<GameObject> d = new List<GameObject>();
        foreach(Enemy enemy in enem)
        {
            d.Add(enemy.gameObject);
        }
        return d;
    }
}
