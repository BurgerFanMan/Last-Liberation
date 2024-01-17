using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurretButtons : MonoBehaviour
{
    [SerializeField] TMP_Text _currencyDisplay;
    [SerializeField] Button _turretButton;
    [SerializeField] TurretInfo _turret;

    private void Start()
    {
        _currencyDisplay.text = $"{_turret.cost}";        
    }

    private void Update()
    {
        if (Money.money < _turret.cost)
            _turretButton.interactable = false;
        else
            _turretButton.interactable = true;
    }
}
