using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IFireBullets : ICanBeUpgraded
{
    [Header("Projectile Settings")]
    public GameObject bulletPrefab;
    [SerializeField] protected bool upgradeBullets;
    [SerializeField] protected int bulletUpgradeIndex = 2;
    public List<string> ignoreTags;

    protected List<Bullet> allBullets = new List<Bullet>();
    protected List<Bullet> freeBullets = new List<Bullet>();

    public void SpawnBullet(Vector3 position, Quaternion rotation, Vector3 targetPosition)
    {
        Bullet bullet;
        //check if there are bullets in the pool, if yes activate them, if no spawn a new one
        if (freeBullets.Count > 0)
        {
            bullet = freeBullets[0];

            bullet.gameObject.SetActive(true);
            bullet.OnSpawn(position, rotation, targetPosition);

            freeBullets.Remove(bullet);

            return;
        }

        bullet = Instantiate(bulletPrefab, position, rotation).GetComponent<Bullet>();

        bullet.OnSpawn(position, rotation, targetPosition);
        bullet.fireBullets = this;

        allBullets.Add(bullet);
        
    }
    public void SpawnBullet(Vector3 position, Quaternion rotation)
    {
        Bullet bullet;
        //check if there are bullets in the pool, if yes activate them, if no spawn a new one
        if(freeBullets.Count > 0)
        {
            bullet = freeBullets[0];

            bullet.gameObject.SetActive(true);
            bullet.OnSpawn(position, rotation);

            freeBullets.Remove(bullet);

            return;
        }

        bullet = Instantiate(bulletPrefab, position, rotation).GetComponent<Bullet>();

        bullet.OnSpawn(position, rotation);
        bullet.fireBullets = this;

        allBullets.Add(bullet);
    }

    public void DestroyBullet(Bullet bullet)
    {
        freeBullets.Add(bullet);

        bullet.gameObject.SetActive(false);
    }
}
