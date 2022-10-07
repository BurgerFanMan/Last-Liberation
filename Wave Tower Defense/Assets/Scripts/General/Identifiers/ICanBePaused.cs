using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICanBePaused : MonoBehaviour
{
    protected float _timeScale = 1f;
    protected bool _paused;

    protected virtual void Awake()
    {

    }
    public void ChangeTime(float newTime)
    {
        _timeScale = newTime;
        if(newTime == 0f)
        {
            _paused = true;
        }
        else
        {
            _paused = false;
        }

        UpdateTime();
    }
    protected virtual void UpdateTime()
    {

    }
}
