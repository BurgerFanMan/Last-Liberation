using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ICanBeUpgraded
{
    public float _speed;
    [SerializeField] float _damage;
    [SerializeField] ParticleSystem _hitEffect;
    [SerializeField] float _range;

    [Header("Readonly")]
    public IFireBullets _fireBullets;

    Vector3 startPoint;
    private void Start()
    {
        startPoint = transform.position;
    }
    void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime * _timeScale;

        if (transform.position.y < -1f || Vector3.Distance(startPoint, transform.position) >= _range * upgradeLevel[0]) 
        {
            DestroyBullet();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<ICanTakeDamage>().DealDamage(_damage * upgradeLevel[1]);
        }
        List<string> ignore = _fireBullets.IgnoreTags();
        if (!ignore.Contains(collision.gameObject.tag))
        {
            DestroyBullet(transform.position);
        }
    }

    void DestroyBullet(Vector3 hitPosition)
    {
        if (_hitEffect != null)
            Instantiate(_hitEffect, hitPosition, Quaternion.identity);
        _fireBullets.DestroyBullet(this);
    }

    void DestroyBullet()
    {
        _fireBullets.DestroyBullet(this);
    }
}
