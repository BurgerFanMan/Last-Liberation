using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterface : MonoBehaviour
{
    [SerializeField] PauseManager _pauseManager;
    [SerializeField] GameObject _shopMenu;

    private bool _shopMenuOpen = false;
    private void Awake()
    {
        _shopMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(InputManager.GetValue("shop_togglevisibility")) && (!Pause.inPauseMenu))
        {
            ShopMenuClicked();
        }
    }

    //actives/deactivates the shop menu while pausing/unpausing the game
    public void ShopMenuClicked()
    {
        if (_shopMenu == null) return;

        _shopMenuOpen = !_shopMenuOpen;
        _shopMenu.SetActive(_shopMenuOpen);

        if (_shopMenuOpen)
            _pauseManager.PauseGame();
        else
            _pauseManager.UnpauseGame();
    }
}
