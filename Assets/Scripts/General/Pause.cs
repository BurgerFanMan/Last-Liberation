using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [Tooltip("Should the unpause button be the same as the pause button?")]
    public bool _unpauseWithKey = true; //Should the unpause button be the same as the pause button?
    public KeyCode _pauseButton = KeyCode.Escape;


    [Range(0f, 5f)]
    public float _defaultTimeScale = 1f;
    [Range(0f, 0.1f)]
    public float _pauseTimeScale = 0f;

    [Range(0f, 0.1f)]
    public float _shopTimeScale = 0.01f;

    [Header("Read Only")]
    [Range(0f, 100f)]
    public float _timeScale; //To be accessed by other scripts.

    public bool paused = false;

    public bool nonPauseMenu = false;

    private PauseBroker pauseBroker;

    private void Awake()
    {
        _timeScale = _defaultTimeScale;

        pauseBroker = GetComponent<PauseBroker>();
    }
    
    private void Update()
    {
        if (paused && !nonPauseMenu && Input.GetKeyDown(_pauseButton))
        {
            Unpause();
        }

        else if (!paused && Input.GetKeyDown(_pauseButton))
        {
            PauseAction(true);
        }
    }

    public void Unpause()
    {
        _timeScale = _defaultTimeScale;

        paused = false;
        nonPauseMenu = false;

        pauseBroker.UnPause();
    }

    public void PauseAction(bool openPauseMenu)
    {
        _timeScale = _pauseTimeScale;
        if (!openPauseMenu)
        {
            nonPauseMenu = true;
        }

        paused = true;

        pauseBroker.Pause();
    }
}
