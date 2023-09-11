using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFireBullets : ICanBeUpgraded
{
    protected List<Bullet> bullets = new List<Bullet>();
    protected List<Bullet> disposedBullets = new List<Bullet>();
    [SerializeField] protected List<string> ignoreTags;
    [SerializeField] protected bool upgradeBullets;
    [SerializeField] protected int bulletUpgradeIndex = 2;

    public List<string> IgnoreTags()
    {
        return ignoreTags;
    }

    public void DestroyBullet(Bullet bullet)
    {
        if (bullets.Contains(bullet) && !disposedBullets.Contains(bullet))
        {
            Destroy(bullet.gameObject);
            bullets.Remove(bullet);
            disposedBullets.Add(bullet);
        }
        else if(!disposedBullets.Contains(bullet))
        {
            Debug.Log($"Bullet not found. Bullet: {bullet}");
        }
    }
}
