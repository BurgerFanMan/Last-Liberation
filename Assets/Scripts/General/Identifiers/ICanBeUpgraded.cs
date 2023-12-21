using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICanBeUpgraded : MonoBehaviour
{
    [SerializeField] protected List<float> _upgradeLevel = new List<float>();

    public virtual void Upgrade(float amount, int index)
    {
        _upgradeLevel[index] = amount;
    }

    public float GetUpgradeLevelValue(int index)
    {
        return _upgradeLevel[index];
    }
}
