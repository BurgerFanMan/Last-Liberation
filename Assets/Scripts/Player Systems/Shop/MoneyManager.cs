using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _moneyDisplay;
    [SerializeField] float _enemyBounty;

    public float startMoney;

    private void Awake()
    {
        Money.money = startMoney;
    }

    private void Update()
    {
        _moneyDisplay.text = Mathf.Round(Money.money).ToString();
    }
}

public static class Money
{
    public static float money = 0f;
}
