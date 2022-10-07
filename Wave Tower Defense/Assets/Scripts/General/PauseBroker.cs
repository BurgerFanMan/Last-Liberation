using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBroker : MonoBehaviour
{
    [SerializeField] Pause _pauseManager;

    [Header("UI Management")]
    [SerializeField] GameObject _pauseMenu;

    private float timeScale;
    private float tempTimeScale;

    private void Awake()
    {
        timeScale = _pauseManager._timeScale;
    }

    private void Update()
    {
        timeScale = _pauseManager._timeScale;
        if(tempTimeScale != timeScale)
        {
            ChangeValues();
            if(_pauseManager.paused)
            {
                _pauseMenu.SetActive(true);
            }
            else
            {
                _pauseMenu.SetActive(false);
            }
        }
        tempTimeScale = timeScale;
    }

    void ChangeValues()
    {
        ICanBePaused[] pausable = Object.FindObjectsOfType<ICanBePaused>();
        foreach (ICanBePaused p in pausable)
        {
            p.ChangeTime(timeScale);
        }
    }
}
