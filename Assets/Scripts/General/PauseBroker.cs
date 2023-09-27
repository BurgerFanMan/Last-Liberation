using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PauseBroker : MonoBehaviour
{
    [SerializeField] Pause _pauseManager;

    [Header("UI Management")]
    [SerializeField] GameObject _pauseMenu;

    private List<ICanBePaused> pausable = new List<ICanBePaused>();

    private float timeScale;

    private void Awake()
    {
        timeScale = _pauseManager._timeScale;

        pausable = FindObjectsOfType<ICanBePaused>().ToList();
    }

    void ChangeValues()
    {
        foreach (ICanBePaused p in pausable)
        {
            p.ChangeTime(timeScale);
        }
    }

    public void Pause() 
    {
        if (!_pauseManager.nonPauseMenu)
        {
            _pauseMenu.SetActive(true);
        }

        timeScale = _pauseManager._timeScale;

        ChangeValues();
    }

    public void UnPause()
    {
        _pauseMenu.SetActive(false);

        timeScale = _pauseManager._timeScale;

        ChangeValues();
    }
}
