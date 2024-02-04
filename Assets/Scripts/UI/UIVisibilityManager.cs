using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIVisibilityManager : MonoBehaviour
{
    [SerializeField] GameObject _canvas;
    [SerializeField] GameObject _cursorFollowers;

    void Update()
    {
        if (Input.GetKeyDown(InputManager.GetValue("ui_togglevisibility")))
        {
            _canvas.SetActive(!_canvas.activeSelf);
        }
    }

    public void ChangeBuildMode(bool enter) 
    {   
        if (_cursorFollowers != null)
            _cursorFollowers.SetActive(!enter);

        Cursor.visible = !enter;
    }
}
