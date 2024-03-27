using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string _mainSceneName;
    [SerializeField] GameObject _loadingScreen;

    public void StartPressed()
    {
        if (_loadingScreen != null)
            _loadingScreen.SetActive(true);

        SceneManager.LoadSceneAsync(_mainSceneName);
    }

    public void ExitPressed()
    {
        Application.Quit();
    }
}
