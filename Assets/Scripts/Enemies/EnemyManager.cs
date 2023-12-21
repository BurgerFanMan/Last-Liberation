using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyManager : MonoBehaviour
{
    [Header("Waves")] 
    [Range(0.001f, 0.1f)]
    [SerializeField] float _difficultyPerWave;
    [SerializeField] float _enemiesPerDifficulty; //Multiplied with wave difficulty to get number of enemies to spawn.
    [SerializeField] int _megaWaveFreq; //How often a difficult wave occurs.
    [SerializeField] float _megaWaveDifMult;
    [Range(0f, 20f)]
    [SerializeField] float _timeBetweenWaves;
    [SerializeField] Animator _waveNumbAnim;
    [SerializeField] AudioSource _alarmSource;
    [SerializeField] AudioSource _megaAlarmSource;
    [SerializeField] TextMeshProUGUI _waveUI;

    [Header("Spawning")]
    [Range(0f, 80f)]
    [SerializeField] float _randomSpawnOffset; //Moves the enemy backwards x, where x is a random value between 0 and _randomSpawnOffset.
    [SerializeField] Transform _spawnPointParent;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] GameObject _enemyDestroyPrefab;
    [SerializeField] GameObject _enemyPrefab;

    public List<Enemy> enemies = new List<Enemy>();

    float _waveDifficulty; 
    int _waveNumber = 0; //sort this out
    bool _ongoingWave = true, _waitingForWave;
    float _timePassed;

    private MoneyManager _moneyManager;
    private BuildingManager _buildingManager;

    private void Start()
    {
        _alarmSource = _alarmSource == null ? GetComponent<AudioSource>() : _alarmSource;
        _megaAlarmSource = _megaAlarmSource == null ? GetComponent<AudioSource>() : _megaAlarmSource;

        _moneyManager = FindObjectOfType<MoneyManager>();
        _buildingManager = FindObjectOfType<BuildingManager>();
    }

    void Update()
    {
        if (enemies.Count != 0)
            return;

        if (_ongoingWave)
        {
            _ongoingWave = false;
            _waitingForWave = true;
            _waveNumbAnim.SetBool("waitingForWave", true);
        }
        else if (_waitingForWave)
        {
            _timePassed += Pause.adjTimeScale;
            if (_timePassed > _timeBetweenWaves)
            {
                IncreaseWave();
            }
        }
    }

    public void IncreaseWave()
    {
        _waveNumbAnim.SetBool("waitingForWave", false);  

        _timePassed = 0f;
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

        _waveUI.text = _waveNumber.ToString();

        SpawnEnemies((int)(_waveDifficulty * _enemiesPerDifficulty));
    }

    public void KillEnemy(Enemy enemyToKill)
    {
        for (int i = enemies.Count; i > 0; i--)
        {
            if (enemies[i-1] == enemyToKill)
            {
                enemies.Remove(enemies[i-1]);
                Instantiate(_enemyDestroyPrefab, enemyToKill.transform.position, enemyToKill.transform.rotation);
                Destroy(enemyToKill.gameObject);
                i = -1;

                _moneyManager.EnemyKilled();
            }
        }
        UpdateTargets();
    }
    public void DamageBuilding(float damage, GameObject building)
    {
        _buildingManager.DamageBuilding(building, damage);
        UpdateTargets();
    }

    void SpawnEnemies(int _numbOfSpawns)
    {
        for (int i = _numbOfSpawns; i > 0; i--)
        {
            _spawnPointParent.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 spawnLocat = _spawnPoint.position + (_spawnPoint.forward * Random.Range(0, -_randomSpawnOffset));

            Enemy enemy = Instantiate(_enemyPrefab, spawnLocat, Quaternion.identity).GetComponent<Enemy>();
            enemies.Add(enemy);

            enemy.SetTarget(GetNearestObject(_buildingManager._buildings, enemy.transform.position).position);
            enemy.AssignManager(this);
        }
    }
    void UpdateTargets()
    {
        foreach(Enemy enemy in enemies)
        {
            if(_buildingManager._buildings.Count > 0)
                enemy.SetTarget(GetNearestObject(_buildingManager._buildings, enemy.transform.position).position);
        }
    }

    Transform GetNearestObject(List<GameObject> gos, Vector3 currentPos)
    {
        Transform closest = null;
        float dist = 1000000f;
        foreach (GameObject go in gos)
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
}
