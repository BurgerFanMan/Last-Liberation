using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Bullet : ICanBeUpgraded
{
    [SerializeField] protected float _speed;
    public float damage;
    [SerializeField] protected float _range = Mathf.Infinity;
    [SerializeField] protected float _explosionRadius = 0f;
    [SerializeField] protected float _targetRandomness = 0f;
    [SerializeField] protected string _tagToHit = "Enemy";
    [SerializeField] protected GameObject _hitEffect;
    [SerializeField] protected bool _rotateHitEffect; //changes the rotation of the hit effect to match the hit direction 
    
    [Header("Readonly")]
    public IFireBullets fireBullets;

    protected Vector3 targetPoint;
    protected Vector3 startPoint;

    public virtual void OnSpawn(Vector3 position, Quaternion rotation, Vector3 targetPosition)
    {
        targetPoint = targetPosition;
        targetPoint += new Vector3(Random.Range(-_targetRandomness, _targetRandomness), 0f, Random.Range(-_targetRandomness, _targetRandomness));
        OnSpawn(position, rotation);
    }
    public virtual void OnSpawn(Vector3 position, Quaternion rotation)
    {
        startPoint = position;

        transform.SetPositionAndRotation(position, rotation);
    }

    void Update()
    {
        Move();

        //checks range
        if (transform.position.y < -1f || Vector3.Distance(startPoint, transform.position) >= _range * _upgradeLevel[0]) 
        {
            DestroyBullet();
        }
    }

    //checks if the hit object is an enemy and/or should be ignored
    private void OnCollisionEnter(Collision collision)
    {
        if (_explosionRadius == 0)
        {
            if (collision.gameObject.tag == _tagToHit)
            {
                collision.gameObject.GetComponent<ICanTakeDamage>().DealDamage(damage * _upgradeLevel[1]);
            }
        }
        else
        {
            ApplyExplosionDamage(transform.position);
        }

        if (!fireBullets.ignoreTags.Contains(collision.gameObject.tag))
        {
            DestroyBullet(transform.position);
        }
    }

    protected virtual void Move()
    {
        transform.position += _speed * Pause.adjTimeScale * transform.forward;
    }

    //tells firer to destroy bullet and spawns a hit effect
    void DestroyBullet(Vector3 hitPosition)
    {
        if (_hitEffect != null)
        { 
            GameObject hitEffect = Instantiate(_hitEffect, hitPosition, Quaternion.identity);

            hitEffect.transform.rotation = _rotateHitEffect ? transform.rotation : hitEffect.transform.rotation;
        }

        fireBullets.DestroyBullet(this);
    }
    //just tells firer to destroy bullet
    void DestroyBullet()
    {
        fireBullets.DestroyBullet(this);
    }
    void ApplyExplosionDamage(Vector3 position)
    {
        List<Collider> hitColliders = Physics.OverlapSphere(position, _explosionRadius, 1 << 8  ).ToList();

        for (int i = 0; i < hitColliders.Count; i++)
        {
            Enemy enemy = hitColliders[i].GetComponent<Enemy>();

            enemy.DealDamage(damage * _upgradeLevel[1]);
        }
    }
}
