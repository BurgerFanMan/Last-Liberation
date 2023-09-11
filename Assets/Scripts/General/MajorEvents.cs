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
        _loseScreen.SetActive(true);
        _loseScreen.GetComponent<Animator>().SetBool("Lose", true);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
