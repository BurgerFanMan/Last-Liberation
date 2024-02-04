using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Time Scale Presets")]
    [Range(0f, 5f)]
    public float _defaultTimeScale = 1f;
    [Range(0f, 0.1f)]
    public float _pauseTimeScale = 0f;
    [Range(0f, 1f)]
    public float _slowTimeScale = 0.02f;

    [Header("UI Management")]
    [SerializeField] GameObject _pauseMenu;

    [Header("Read Only")]
    [Range(0f, 100f)]
    [SerializeField] float timeScale; //for debugging

    private void Awake()
    {
        ClosePauseMenu();
    }
    
    private void Update()
    {
        if(!Input.GetKeyDown(InputManager.GetValue("pause_togglepause"))) return; 

        if (Pause.isPaused && Pause.inPauseMenu) //if the pause menu is open, close it and unpause
        {
            ClosePauseMenu();
        }
        else if (!Pause.isPaused)
        {
            OpenPauseMenu();
        }

        timeScale = Pause.timeScale;
    }

    //opens/closes pause menu and pauses/unpauses game 
    public void OpenPauseMenu()
    {
        PauseGame();

        Pause.inPauseMenu = true;

        _pauseMenu.SetActive(true);
    }
    public void ClosePauseMenu()
    {
        UnpauseGame();

        _pauseMenu.SetActive(false);
    }


    public void PauseGame()
    {
        Pause.timeScale = _pauseTimeScale;

        Cursor.visible = true;

        Pause.isPaused = true;
    }
    public void UnpauseGame()
    {
        Pause.timeScale = Pause.isSlowed ? _slowTimeScale : _defaultTimeScale;

        Cursor.visible = !SharedVariables.inBuildMode;

        Pause.inPauseMenu = false;
        Pause.isPaused = false;
    }

    public void SlowGame()
    {
        Pause.timeScale = _slowTimeScale;

        Pause.isSlowed = true;
    }
    public void UnslowGame()
    {
        Pause.timeScale = _defaultTimeScale;

        Pause.isSlowed = false;
    }
}
public static class Pause
{
    public static float timeScale;
    public static float adjTimeScale { get { return timeScale * Time.deltaTime; } }
    public static bool inPauseMenu = false;
    public static bool isPaused = false;
    public static bool isSlowed = false;
}

