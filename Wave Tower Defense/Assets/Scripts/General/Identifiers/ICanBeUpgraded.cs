using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ICanBeUpgraded : ICanBePaused
{
    [SerializeField] protected List<float> upgradeLevel = new List<float>();
    protected int upgradedTimes = 0;
    public virtual void Upgrade(float amount, int index)
    {
        upgradeLevel[index] = amount;
        upgradedTimes += 1;
    }

    public float GetUpgradeLevel(int index)
    {
        return upgradeLevel[index];
    }
}
