using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] Money _moneyManager;
    [SerializeField] GameObject _moneyWarning;
    [SerializeField] float _timeToWarningFade;

    public void ItemBought(float cost)
    {
        _moneyManager.DecreaseMoney(cost);
    }

    public void BuyItem(ShopItem shopItem)
    {
        if(shopItem._currentLevel < shopItem._levels && shopItem._costs[shopItem._currentLevel] <= _moneyManager._money)
        {
            shopItem.Buy();
        }
        else if(shopItem._costs[shopItem._currentLevel] > _moneyManager._money && _moneyWarning != null)
        {
            _moneyWarning.SetActive(true);
            CancelInvoke("ResetWarning");
            Invoke("ResetWarning", _timeToWarningFade);
        }
    }

    void ResetWarning()
    {
        _moneyWarning.SetActive(false);
    }
}
