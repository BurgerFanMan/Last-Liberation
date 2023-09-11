using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopInterface : ICanBePaused
{
    
    [SerializeField] Pause _pause;
    [SerializeField] KeyCode _shopButton;
    [SerializeField] GameObject _shopMenu;

    private bool shopMenuOpen = false;
    protected override void Awake()
    {
        _shopMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(_shopButton) && (!_paused || shopMenuOpen))
        {
            ShopMenuClicked();
        }
    }

    public void ShopMenuClicked()
    {
        shopMenuOpen = !shopMenuOpen;
        if(_shopMenu != null)
            _shopMenu.SetActive(shopMenuOpen);

        if (shopMenuOpen == true)
            _pause.PauseAction(false);
        else
            _pause.Unpause();
    }
}
