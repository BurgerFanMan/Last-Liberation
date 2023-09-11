using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltips : MonoBehaviour
{
    [SerializeField] GameObject _toolTip;
    [SerializeField] GameObject _checkMark;

    bool _tooltipOpen = true;

    public void ChangeTooltipState()
    {
        if (_tooltipOpen)
        {
            _tooltipOpen = false;

            _toolTip.SetActive(false);
            _checkMark.SetActive(false);
        }
    }
    
}
