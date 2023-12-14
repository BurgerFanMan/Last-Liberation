using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFireBullets : ICanBeUpgraded
{
    [Header("Projectile Settings")]
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected bool upgradeBullets;
    [SerializeField] protected int bulletUpgradeIndex = 2;
    [SerializeField] protected List<string> ignoreTags;

    protected List<Bullet> allBullets = new List<Bullet>();
    protected List<Bullet> freeBullets = new List<Bullet>();

    public List<string> IgnoreTags()
    {
        return ignoreTags;
    }

    public void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        if(freeBullets.Count > 0)
        {
            freeBullets[0].gameObject.SetActive(true);
            freeBullets[0].transform.position = position;
            freeBullets[0].transform.rotation = rotation;

            freeBullets.Remove(freeBullets[0]);

            return;
        }
        Bullet bullet = Instantiate(bulletPrefab, position, rotation).GetComponent<Bullet>();
        allBullets.Add(bullet);
        bullet._fireBullets = this;
        bullet.ChangeTime(_timeScale);
    }
    public void DestroyBullet(Bullet bullet)
    {
        freeBullets.Add(bullet);

        bullet.gameObject.SetActive(false);
    }

    protected override void UpdateTime()
    {
        foreach (Bullet bullet in allBullets)
            bullet.ChangeTime(_timeScale);
    }
}
