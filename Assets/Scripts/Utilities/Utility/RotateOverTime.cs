using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [Header("General")]
    [SerializeField] Vector3 _rotation;
    [SerializeField] float _speed = 1f;

    void Update()
    {
        transform.rotation *= Quaternion.Euler(_rotation * _speed * Time.deltaTime);
    }
}
