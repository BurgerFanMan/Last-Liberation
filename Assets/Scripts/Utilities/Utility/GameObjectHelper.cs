using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectHelper : MonoBehaviour
{
    [SerializeField] GameObject _target;
    public void SetActive()
    {
        _target.SetActive(!_target.activeSelf);
    }
}
