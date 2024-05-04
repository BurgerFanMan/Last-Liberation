using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MajorEvents : MonoBehaviour
{
    [SerializeField] GameObject _loseScreen;
    [SerializeField] float _timeToLose;
    public void Lose()
    {
        Invoke("LoadMenu", _timeToLose);

        FindObjectOfType<PauseManager>().PauseGame();

        _loseScreen.SetActive(true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
