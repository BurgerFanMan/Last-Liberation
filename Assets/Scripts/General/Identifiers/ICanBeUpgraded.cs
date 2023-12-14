using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICanBeUpgraded : MonoBehaviour
{
    [SerializeField] protected List<float> _upgradeLevel = new List<float>();

    protected int _upgradedTimes = 0;

    public virtual void Upgrade(float amount, int index)
    {
        _upgradeLevel[index] = amount;
        _upgradedTimes += 1;
    }

    public float GetUpgradeLevelValue(int index)
    {
        return _upgradeLevel[index];
    }
}
