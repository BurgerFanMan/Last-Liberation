using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnInput : MonoBehaviour
{
    [SerializeField] KeyCode _key;
    [SerializeField] KeyPressType _keyPressType;
    [SerializeField] UnityEvent _onKeyPressed;

    private bool _up, _down;
    private enum KeyPressType
    {
        down,
        hold,
        up
    }

    private void Start()
    {
        if(_keyPressType == KeyPressType.down)
        {
            _down = true;
        }        
        else if(_keyPressType == KeyPressType.up)
        {
            _up = true;
        }
    }
    void Update()
    {
        bool down = Input.GetKeyDown(_key);
        bool up = Input.GetKeyUp(_key);
        bool hold = Input.GetKey(_key);

        if ((_up && up) || (_down && down) || hold)
            _onKeyPressed.Invoke();
    }
}
