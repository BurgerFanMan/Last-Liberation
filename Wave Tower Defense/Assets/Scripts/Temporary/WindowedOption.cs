using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowedOption : MonoBehaviour
{
    [SerializeField] bool startInWindowed = false;
    bool _windowed = false;

    int width, height;

    private void Start()
    {
        width = Screen.currentResolution.width;
        height = Screen.currentResolution.height;

        if (startInWindowed)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
    public void Switch()
    {
        if (!_windowed)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            _windowed = true;
        }
        else
        {
            Screen.SetResolution(width, height, FullScreenMode.FullScreenWindow);
            _windowed = false;
        }
    }
}
