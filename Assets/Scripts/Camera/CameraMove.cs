using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform _cameraParent;
    [SerializeField] KeyCode _lockCameraKey;
    [SerializeField] float _speed;

    private Vector3 origin = new Vector3(0f, 0f, 0f);

    void Update()
    {
        if (!Pause.isPaused && !Input.GetKey(_lockCameraKey))
        {
            Vector3 _targetPos;
            _targetPos = RayStore.hitPoint;
            _targetPos.y = 0;
            Vector3 newPos = ((origin + _targetPos) / 2);

            Vector3 movement = Vector3.Lerp(_cameraParent.position, newPos, _speed * Time.deltaTime);
            _cameraParent.position = movement;
        }
    }
}
