using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    [Header("General")]
    [SerializeField] ICanBeUpgraded _upgradeTarget;
    public List<float> costs = new List<float>();

    [Header("Levels")]   
    [SerializeField] int _numberOfLevels = 1;
    [SerializeField] string secondaryDescription = "";
    public int currentLevel;

    [Header("Upgrade")]
    [Tooltip("How to affect the current value.")]
    [SerializeField] Operator _operator;
    [SerializeField] bool _affectOriginal;
    [SerializeField] List<float> _values = new List<float>();
    [SerializeField] int index;

    [Header("References")]
    [SerializeField] Button _buyButton;
    [SerializeField] Text _costDisplay;

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
        originalValue = _upgradeTarget.GetUpgradeLevelValue(index); 
        currentLevel = 0;

        shopManager = FindObjectOfType<Shop>();

        _buyButton = _buyButton == null ? GetComponent<Button>() : _buyButton;
        _costDisplay = _costDisplay == null ? GetComponentsInChildren<Text>()[0] : _costDisplay;

        _buyButton.onClick.AddListener(delegate { shopManager.BuyItem(this); });
        _costDisplay.text = costs[currentLevel].ToString();

        _buyButton.interactable = _prerequisite == null;
    }

    private void Update()
    {   
        //check if the prerequisite is fulfilled
        if (_prerequisite == null || prequisiteFulfilled)
            return;
        if (_prerequisite.currentLevel >= _prerequisiteLevel)
        {
            _buyButton.interactable = true;
            prequisiteFulfilled = true;
        }
    }

    public void Buy()
    {
        shopManager.ItemBought(costs[currentLevel]);      
        BuyAction();

        currentLevel += 1;
        if (currentLevel != _numberOfLevels)
            _costDisplay.text = costs[currentLevel].ToString();
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
        float newValue = _values[currentLevel];
        float value = _affectOriginal ? originalValue : _upgradeTarget.GetUpgradeLevelValue(index);
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
