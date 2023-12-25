using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : IFireBullets
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
    Vector3 _startRotation;
    Quaternion _lookRot, _bulletRot;

    public void SetValues(GameObject bulletPrefabPass, float turnSpeed, float rotClamp)
    {
        bulletPrefab = bulletPrefabPass; _baseTurnSpeed = turnSpeed;

        _startRotation = _sniperBody.rotation.eulerAngles;

        _rotationClamp = rotClamp;
        _rotClampMin = _startRotation.y - _rotationClamp; 
        _rotClampMax = _startRotation.y + _rotationClamp;
    }

    public void FacePosition(Vector3 position)
    {
        _turnSpeed = _baseTurnSpeed * Pause.adjTimeScale;

        _lookRot = Quaternion.LookRotation(position - transform.position);
        _bulletRot = Quaternion.LookRotation(position - _gunExit.position);

        Vector3 rotation = _lookRot.eulerAngles;
        rotation.y = GetRotation(rotation.y); //adjusts rotation

        if (rotation.y < _rotClampMax && rotation.y > _rotClampMin)
        {
            rotation = Quaternion.Lerp(_sniperBody.rotation,  _lookRot, _turnSpeed).eulerAngles;
            _sniperBody.rotation = Quaternion.Euler(_startRotationOffset.x, rotation.y + _startRotationOffset.y, 0f);
        }
        Vector3 gunRotation = Quaternion.Lerp(_sniperGun.rotation, _lookRot, _turnSpeed).eulerAngles;
        _sniperGun.localRotation = Quaternion.Euler(gunRotation.x, 0f, 0f);
    }

    public bool Shoot()
    {
        Vector3 rotation = _lookRot.eulerAngles;
        rotation.y = GetRotation(rotation.y);
        if (rotation.y < _rotClampMax && rotation.y > _rotClampMin)
        {
            Vector3 spawnOffset = _gunExit.forward * 0.4f;
            SpawnBullet(_gunExit.position + spawnOffset, _bulletRot);

            return true;
        }

        return false;
    }

    float GetRotation(float rotation)
    {
        rotation = 
            (rotation > _rotClampMax && rotation - 360f > _rotClampMin) ? rotation - 360f :
            (rotation < _rotClampMin && rotation + 360f < _rotClampMax) ? rotation + 360f :
            rotation;

        return rotation;
    }
}
