using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ICanBeUpgraded
{
    [SerializeField] private float _speed;
    [SerializeField] private float _damage;
    [SerializeField] private float _range;
    [SerializeField] private GameObject _hitEffect;
    
    [Header("Readonly")]
    public IFireBullets fireBullets;
    
    private Vector3 startPoint;

    public void OnSpawn(Vector3 position, Quaternion rotation)
    {
        transform.SetPositionAndRotation(position, rotation);
    }

    void Update()
    {
        transform.position += _speed * Pause.adjTimeScale * transform.forward;

        //checks range
        if (transform.position.y < -1f || Vector3.Distance(startPoint, transform.position) >= _range * _upgradeLevel[0]) 
        {
            DestroyBullet();
        }
    }

    //checks if the hit object is an enemy and/or should be ignored
    private void OnCollisionEnter(Collision collision)
    {   
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<ICanTakeDamage>().DealDamage(_damage * _upgradeLevel[1]);
        }
        if (!fireBullets.ignoreTags.Contains(collision.gameObject.tag))
        {
            DestroyBullet(transform.position);
        }
    }


    //tells firer to destroy bullet and spawns a hit effect
    void DestroyBullet(Vector3 hitPosition)
    {
        if (_hitEffect != null)
            Instantiate(_hitEffect, hitPosition, Quaternion.identity);

        fireBullets.DestroyBullet(this);
    }
    //just tells firer to destroy bullet
    void DestroyBullet()
    {
        fireBullets.DestroyBullet(this);
    }
}
