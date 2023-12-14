using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Tooltip("Should the unpause button be the same as the pause button?")]
    public bool _unpauseWithKey = true; //Should the unpause button be the same as the pause button?
    public KeyCode _pauseButton = KeyCode.Escape;

    [Range(0f, 5f)]
    public float _defaultTimeScale = 1f;
    [Range(0f, 0.1f)]
    public float _pauseTimeScale = 0f;

    [Header("UI Management")]
    [SerializeField] GameObject _pauseMenu;

    [Header("Read Only")]
    [Range(0f, 100f)]
    public float timeScale; //for debugging

    private void Awake()
    {
        ClosePauseMenu();
    }
    
    private void Update()
    {
        if(!Input.GetKeyDown(_pauseButton)) return; 
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

        Pause.isPaused = true;
    }
    public void UnpauseGame()
    {
        Pause.timeScale = _defaultTimeScale;

        Pause.inPauseMenu = false;
        Pause.isPaused = false;
    }
}
public static class Pause
{
    public static float timeScale;
    public static float adjTimeScale { get { return timeScale * Time.deltaTime; } }
    public static bool inPauseMenu = false;
    public static bool isPaused = false;
}