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
    [SerializeField] float _randomSpawnOffset; //Moves the enemy backwards or forwards a random value between -_randomSpawnOffset and _randomSpawnOffset.
    [SerializeField] Transform _spawnPointParent;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] GameObject _enemyDestroyPrefab;
    [SerializeField] GameObject _enemyPrefab;

    [Header("Money")]
    [SerializeField] float _bounty = 1f;
    [SerializeField] float _waveBonus = 5f;
    [SerializeField] bool _increaseBonusWithDifficulty = true;

    public List<Enemy> enemies = new List<Enemy>();

    float _waveDifficulty; 
    int _waveNumber = 0; //sort this out- starting wave delay
    float _timePassed;

    private BuildingManager _buildingManager;

    private void Start()
    {
        _alarmSource = _alarmSource == null ? GetComponent<AudioSource>() : _alarmSource;
        _megaAlarmSource = _megaAlarmSource == null ? GetComponent<AudioSource>() : _megaAlarmSource;

        _buildingManager = FindObjectOfType<BuildingManager>();

        SharedVariables.enemyManager = this;
    }

    void Update()
    {
        if (enemies.Count != 0)
            return;

        _timePassed += Pause.adjTimeScale;
        if (_timePassed > _timeBetweenWaves)
        {
            _timePassed = 0f;

            IncreaseWave();
        }
    }

    public void IncreaseWave()
    {
        _waveNumber += 1;

        _waveDifficulty = (_difficultyPerWave * _waveNumber) + Mathf.Pow(1.1f, _waveNumber);
        _waveDifficulty *= _waveNumber % _megaWaveFreq == 0 ? _megaWaveDifMult : 1f;

        _waveUI.text = _waveNumber.ToString();
        _waveNumbAnim.SetBool("waitingForWave", false);

        SpawnEnemies((int)(_waveDifficulty * _enemiesPerDifficulty));
    }
    public void OnWaveEnded()
    {
        Money.money += _increaseBonusWithDifficulty ? _waveBonus * _waveDifficulty : _waveBonus;

        _waveNumbAnim.SetBool("waitingForWave", true);
    }

    public void KillEnemy(Enemy enemyToKill)
    {
        if (!enemies.Contains(enemyToKill))
            return;

        Instantiate(_enemyDestroyPrefab, enemyToKill.transform.position, enemyToKill.transform.rotation);

        enemies.Remove(enemyToKill);
        Destroy(enemyToKill.gameObject);

        Money.money += _bounty;
    }
    public void DamageBuilding(float damage, GameObject building)
    {
        _buildingManager.DamageBuilding(building, damage);
        UpdateTargets();
    }

    void SpawnEnemies(int numbOfSpawns)
    {
        for (int i = 0; i < numbOfSpawns; i++)
        {
            _spawnPointParent.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            Vector3 spawnLocat = _spawnPoint.position + (_spawnPoint.forward * Random.Range(-_randomSpawnOffset, _randomSpawnOffset));

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
