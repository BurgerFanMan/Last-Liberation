using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtons : MonoBehaviour
{
    [SerializeField] Pause _pause;

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ResumeGame()
    {
        if(_pause != null || gameObject.TryGetComponent<Pause>(out _pause))
        {
            _pause._timeScale = _pause._defaultTimeScale;
            _pause.paused = false;
        }
    }

    public void OpenOptions()
    {

    }
}
