using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tabs : MonoBehaviour
{
    [SerializeField] List<GameObject> _tabs;
    [SerializeField] List<Button> _buttons;

    private void Start()
    {
        foreach(Button button in _buttons)
        {
            button.onClick.AddListener(delegate { SwitchTabs(_buttons.IndexOf(button));  } );
        }
    }
    void SwitchTabs(int tabIndex)
    {
        foreach(GameObject go in _tabs)
        {
            go.SetActive(false);
        }
        _tabs[tabIndex].SetActive(true);
    }
}
