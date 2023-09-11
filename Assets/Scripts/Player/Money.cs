using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Money : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyDisplay;
    [SerializeField] float _enemyBounty;

    public float _startMoney;
    public float _money = 0f;

    private bool moneyDisplayNull;

    private void Awake()
    {
        _money = _startMoney;

        if (_moneyDisplay == null)
        {
            moneyDisplayNull = true;
        }
    }

    private void Update()
    {
        if(!moneyDisplayNull)
        {
            _moneyDisplay.text = Mathf.Round(_money).ToString();
        }
    }

    public void DecreaseMoney(float amount)
    {
        _money -= amount;
    }

    public void EnemyKilled()
    {
        _money += _enemyBounty;
    }
}
