using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper : IFireBullets
{
    [Header("Models")]
    [SerializeField] Transform _sniperBody;
    [SerializeField] Transform _sniperGun;

    [Header("Positions & Rotations")]
    [SerializeField] Vector3 _startRotationOffset;
    [SerializeField] Transform _gunExit;
    [SerializeField] float _rotationClamp;
    
    float _rotClampMin, _rotClampMax;
    [Tooltip("Assigned by SniperManager")]
    public float _baseTurnSpeed;

    private float _turnSpeed;
    
    GameObject _bulletPrefab;
    Vector3 _startRotation;
    Quaternion _lookRot, _bulletRot;

    private void CustomAwake()
    {
        _startRotation = _sniperBody.rotation.eulerAngles;
        _rotClampMin = _startRotation.y - _rotationClamp; _rotClampMax = _startRotation.y + _rotationClamp;
    }

    public void SetValues(GameObject bulletPrefab, float turnSpeed,  
        float rotClamp)
    {
        _bulletPrefab = bulletPrefab; _baseTurnSpeed = turnSpeed;
        _rotationClamp = rotClamp;
        CustomAwake();
    }

    public void FacePosition(Vector3 position)
    {
        _turnSpeed = _baseTurnSpeed * _timeScale;

        Vector3 direct = position - transform.position;
        _lookRot = Quaternion.LookRotation(direct);
        _bulletRot = Quaternion.LookRotation(position - _gunExit.position);

        Vector3 rotation = _lookRot.eulerAngles;
        rotation.y = 
            (rotation.y > _rotClampMax && rotation.y - 360 > _rotClampMin) ? rotation.y - 360 : 
            (rotation.y < _rotClampMin && rotation.y + 360 < _rotClampMax) ? rotation.y + 360 :
            rotation.y;

        if (rotation.y < _rotClampMax && rotation.y > _rotClampMin)
        {
            rotation = Quaternion.Lerp(_sniperBody.rotation,
            _lookRot, Time.deltaTime * _turnSpeed).eulerAngles;
            _sniperBody.rotation = Quaternion.Euler(_startRotationOffset.x,
                rotation.y + _startRotationOffset.y, 0f);
        }
        Vector3 gunRotation = Quaternion.Lerp(_sniperGun.rotation,
            _lookRot, Time.deltaTime * _turnSpeed).eulerAngles;
        _sniperGun.localRotation = Quaternion.Euler(gunRotation.x, 0f, 0f);
    }

    public bool Shoot()
    {
        Vector3 rotation = _lookRot.eulerAngles;
        rotation.y =
            (rotation.y > _rotClampMax && rotation.y - 360 > _rotClampMin) ? rotation.y - 360 :
            (rotation.y < _rotClampMin && rotation.y + 360 < _rotClampMax) ? rotation.y + 360 :
            rotation.y;
        if (rotation.y < _rotClampMax && rotation.y > _rotClampMin)
        {
            Vector3 spawnOffset = _gunExit.forward * 0.4f;
            Bullet bullet = Instantiate(_bulletPrefab, _gunExit.position + spawnOffset, _bulletRot)
                .GetComponent<Bullet>();
            bullets.Add(bullet);
            bullet._fireBullets = this;
            bullet.ChangeTime(_timeScale);

            return true;
        }

        return false;
    }
}
