using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : ICanBePaused
{
    [Header("Waves")]
    [SerializeField] AudioSource _alarmSource;
    [SerializeField] AudioSource _megaAlarmSource;
    [SerializeField] TextMeshProUGUI _waveUI;
    [Range(0.001f, 0.1f)]
    [SerializeField] float _difficultyPerWave;
    [SerializeField] float _enemyMult; //Multiplied with wave difficulty to get number of enemies to spawn.
    [SerializeField] int _megaWaveFreq; //How often a difficult wave occurs.
    [SerializeField] float _megaWaveDifMult;
    [Range(0, 20)]
    [SerializeField] float _timeBetweenWaves;
    [SerializeField] Animator _waveNumbAnim;

    [Header("Spawning")]
    [SerializeField] GameObject _enemyPrefab;
    [SerializeField] Transform _spawnPointParent;
    [SerializeField] Transform _spawnPoint;
    [Range(0, 80)]
    [SerializeField] float _randomSpawnOffset; //Moves the enemy backwards x, where x is a
                                               //random value between 0 and _randomSpawnOffset.
    [SerializeField] GameObject _enemyDestroyPrefab;

    public List<Enemy> _enemies = new List<Enemy>();
    List<GameObject> _targets = new List<GameObject>();

    float _waveDifficulty; 
    int _waveNumber;
    bool _ongoingWave = true, _waitingForWave;
    float _timePassed;

    private Money _moneyManager;
    private BuildingManager _buildingManager;

    private void Start()
    {
        if(_alarmSource == null)
        {
            _alarmSource = GetComponent<AudioSource>();
        }
        else if (_megaAlarmSource == null)
        {
            if (GetComponent<AudioSource>() != _alarmSource)
                _megaAlarmSource = GetComponent<AudioSource>();
        }
        _moneyManager = FindObjectOfType<Money>();
        _buildingManager = FindObjectOfType<BuildingManager>();
        _targets = _buildingManager._buildings;
    }

    void Update()
    {
        _waveUI.text = _waveNumber.ToString();

        if(_enemies.Count == 0)
        {
            if (_ongoingWave)
            {
                _ongoingWave = false;
                _waitingForWave = true;
                _waveNumbAnim.SetBool("waitingForWave", true);
            }
            else if (_waitingForWave)
            {
                _timePassed += 1f * Time.deltaTime * _timeScale;
                if(_timePassed > _timeBetweenWaves)
                {
                    IncreaseWave();
                }
            }
        }

        foreach(Enemy enem in _enemies)
        {
            enem.ChangeTime(_timeScale);
        }
    }
    private void LateUpdate()
    {
        _targets = _buildingManager._buildings;
    }

    void SpawnEnemies(int _numbOfSpawns)
    {
        for (int i = _numbOfSpawns; i > 0; i--)
        {
            _spawnPointParent.rotation = Quaternion.Euler(0f, Random.Range(0f, 359.9f), 0f);
            Vector3 spawnLocat = _spawnPoint.position + (_spawnPoint.forward * Random.Range(0, -_randomSpawnOffset));
            Enemy enemy = Instantiate(_enemyPrefab, spawnLocat, Quaternion.identity).GetComponent<Enemy>();
            _enemies.Add(enemy);
            
            enemy.Move(GetNearestObject(_targets, enemy.transform.position).position);
            enemy.AssignManager(this);
        }
    }

    public void IncreaseWave()
    {
        _waveNumbAnim.SetBool("waitingForWave", false);

        _timePassed = 0;
        _waitingForWave = false;
        _ongoingWave = true;

        _waveNumber += 1;
        if (_waveNumber % _megaWaveFreq != 0)
        {
            
            _waveDifficulty = 0.98f * Mathf.Exp(_difficultyPerWave * _waveNumber);
        }
        else
        {
            
            _waveDifficulty = 0.98f * Mathf.Exp(_difficultyPerWave * _waveNumber) * _megaWaveDifMult;
        }

        SpawnEnemies((int)(_waveDifficulty * _enemyMult));
    }

    public void KillEnemy(Enemy enemyToKill)
    {
        for (int i = _enemies.Count; i > 0; i--)
        {
            if (_enemies[i-1] == enemyToKill)
            {
                _enemies.Remove(_enemies[i-1]);
                Instantiate(_enemyDestroyPrefab, enemyToKill.transform.position, enemyToKill.transform.rotation);
                Destroy(enemyToKill.gameObject);
                i = -1;

                _moneyManager.EnemyKilled();
            }
        }
        UpdateTargets();
    }

    Transform GetNearestObject(List<GameObject> gos, Vector3 currentPos)
    {
        Transform closest = null;
        float dist = 1000000f;
        foreach(GameObject go in gos)
        {
            float thisDist = Vector3.Distance(go.transform.position, currentPos);
            if (thisDist < dist)
            {
                dist = thisDist;
                closest = go.transform;
            }
        }
        return closest;
    }

    public void DamageBuilding(float damage, GameObject building)
    {
        _buildingManager.DamageBuilding(building, damage);
        UpdateTargets();
        Invoke("UpdateTargets", 2f);
    }

    void UpdateTargets()
    {
        foreach(Enemy enemy in _enemies)
        {
            if(_targets.Count > 0)
                enemy.Move(GetNearestObject(_targets, enemy.transform.position).position);
        }
    }
}
