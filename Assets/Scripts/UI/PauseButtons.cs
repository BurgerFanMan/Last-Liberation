using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    [SerializeField] PauseManager _pauseManager;

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitToOS()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        if(_pauseManager != null || gameObject.TryGetComponent(out _pauseManager))
        {
            _pauseManager.ClosePauseMenu();
        }
    }
}
