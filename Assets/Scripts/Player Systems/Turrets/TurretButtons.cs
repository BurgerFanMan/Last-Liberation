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

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI _fireRate;
    [SerializeField] TextMeshProUGUI _reloadTime;
    [SerializeField] TextMeshProUGUI _magazineSize;
    [SerializeField] TextMeshProUGUI _damage;
    [SerializeField] TextMeshProUGUI _range;
    [SerializeField] TextMeshProUGUI _angle;

    private void Start()
    {
        _currencyDisplay.text = $"{_turret.cost}";

        UpdateStatDisplays();
    }

    private void Update()
    {
        if (Money.money < _turret.cost)
            _turretButton.interactable = false;
        else
            _turretButton.interactable = true;

        
    }

    void UpdateStatDisplays()
    {
        Turret turret = _turret.turretPrefab.GetComponent<Turret>();
        Bullet bullet = turret.bulletPrefab.GetComponent<Bullet>();

        _fireRate.text = $"{turret.fireRate} RPS";
        _reloadTime.text = $"{turret.reloadTime}s";
        _magazineSize.text = $"{turret.magazineCapacity}";
        _damage.text = $"{bullet.damage}";
        _range.text = $"{turret.maxRange}m";
        _angle.text = $"{turret.angleRange*2f}";
    }
}
