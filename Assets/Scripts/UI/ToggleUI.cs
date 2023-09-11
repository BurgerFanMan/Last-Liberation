using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUI : MonoBehaviour
{
    [SerializeField] KeyCode _toggleUI;
    [SerializeField] GameObject _canvas;
    
    void Update()
    {
        if (Input.GetKeyDown(_toggleUI))
        {
            _canvas.SetActive(!_canvas.activeSelf);
        }
    }
}
