using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipWave : MonoBehaviour
{
    [SerializeField] EnemyManager _enemyManager;
    [SerializeField] Money _moneyManager;
    [SerializeField] float _defaultDelay;
    [SerializeField] float _defaultReward;

    public float _timeScale = 1f;

    bool skipping; float delayTime; float skipTime;

    public void SkipButton()
    {
        SkipDelay(_defaultDelay);
        _moneyManager._money += _defaultReward;
    }
    
    public void Skip(float delay)
    {
        SkipDelay(delay);
    }

    public void Skip(float delay, float reward)
    {
        SkipDelay(delay);
        _moneyManager._money += reward;
    }

    private void Update()
    {
        if (skipping)
        {
            SkipUpdate();
        }
    }

    void SkipUpdate()
    {
        skipTime += Time.deltaTime * _timeScale;
        if(skipTime >= delayTime)
        {
            skipping = false;
            SkipDelay(0f);
        }
    }

    void SkipDelay(float delay)
    {
        if (delay > 0f)
        {
            skipping = true;
            delayTime = delay; skipTime = 0f;
        }
        else
        {
            _enemyManager.IncreaseWave();
        }
    }
}
