using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("General")]
    [SerializeField] ICanBeUpgraded _upgradeTarget;
    public List<float> _costs = new List<float>();

    [Header("Levels")]
    public int _levels = 1;
    public int _currentLevel;
    [SerializeField] string secondaryDescription = "";

    [Header("Upgrade")]
    [Tooltip("How to affect the current value.")]
    [SerializeField] Operator _operator;
    [SerializeField] bool _affectOriginal;
    [SerializeField] List<float> _values = new List<float>();
    [SerializeField] int index;

    [Header("References")]
    public Button _buyButton;
    public Text _costDisplay;

    [Header("Prerequisites")]
    [SerializeField] ShopItem _prerequisite;
    [SerializeField] int _prerequisiteLevel = 1;

    private float originalValue;
    private Shop shopManager;

    private bool prequisiteFulfilled;

    public enum Operator
    {
        replace = 0,
        multiply = 1,
        divide = 2,
        add = 3,
        subtract = 4
    }

    public void Awake()
    {
        originalValue = _upgradeTarget.GetUpgradeLevel(index); 
        _currentLevel = 0;

        shopManager = FindObjectOfType<Shop>();

        if (_buyButton == null)
            _buyButton = GetComponent<Button>();
        _buyButton.onClick.AddListener(delegate { shopManager.BuyItem(this); });

        if (_costDisplay == null)
            _costDisplay = GetComponentsInChildren<Text>()[0];
        _costDisplay.text = _costs[_currentLevel].ToString();

        if(_prerequisite != null)
        {
            _buyButton.interactable = false;
        }
    }

    private void Update()
    {
        if(_prerequisite != null && !prequisiteFulfilled)
        {
            if(_prerequisite._currentLevel >= _prerequisiteLevel)
            {
                _buyButton.interactable = true;
                prequisiteFulfilled = true;
            }
        }
    }

    public void Buy()
    {
        shopManager.ItemBought(_costs[_currentLevel]);      
        BuyAction();

        _currentLevel += 1;
        if (_currentLevel != _levels)
            _costDisplay.text = _costs[_currentLevel].ToString();
        else
        {
            _buyButton.interactable = false;
            _costDisplay.text = "MAX";
        }

        if(secondaryDescription != "")
        {
            GetComponentsInChildren<Text>()[1].text = secondaryDescription;
        }
    }

    public void BuyAction()
    {
        float newValue = _values[_currentLevel];
        float value = _affectOriginal ? originalValue : _upgradeTarget.GetUpgradeLevel(index);
        if (_operator == Operator.multiply)
            newValue *= value;
        else if (_operator == Operator.divide)
            newValue = value / newValue;
        else if (_operator == Operator.add)
            newValue += value;
        else if (_operator == Operator.subtract)
            newValue = value - newValue;

        _upgradeTarget.Upgrade(newValue, index);
    } 
}
