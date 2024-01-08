using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject _moneyWarning;
    [SerializeField] float _timeToWarningFade;

    public void ItemBought(float cost)
    {
        Money.money -= cost;
    }

    public void BuyItem(ShopItem shopItem)
    {
        if(shopItem.costs[shopItem.currentLevel] <= Money.money)
        {
            shopItem.Buy();
        }
        else if(_moneyWarning != null)
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
