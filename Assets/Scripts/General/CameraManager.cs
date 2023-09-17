using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : ICanBePaused
{
    [Header("Cursor")]
    [SerializeField] bool _switchCursor; //False for default cursor
    [SerializeField] Texture2D _cursorImage;
    [SerializeField] Vector2 _cursorOffset;

    [Header("Camera")]
    [SerializeField] Transform _cameraY; //Left and right orbit
    [SerializeField] Transform _cameraX; //Up and down orbit
    [Range(0, 100)]
    [SerializeField] float _orbitSpeedY;
    [Range(0, 5)]
    [SerializeField] float _clampMult;

    [Header("Top-down")]
    [SerializeField] public bool _topDown;
    [SerializeField] float _camXAngle; //Angle of X Rotation when not in top down.

    void Start()
    {
        if(_switchCursor)
            Cursor.SetCursor(_cursorImage, _cursorOffset, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update()
    {
        if (_timeScale == 0f)
            return;
        if(Input.GetKey(KeyCode.Mouse1))
        {
            float camYOrbit = Input.GetAxis("Mouse X") * Time.deltaTime * _orbitSpeedY * 10f;
            camYOrbit = Mathf.Clamp(camYOrbit, _orbitSpeedY * -_clampMult * Time.deltaTime, _orbitSpeedY * _clampMult * Time.deltaTime);
            _cameraY.Rotate(0f, camYOrbit, 0f, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeMode();
        }
    }

    public void ChangeMode()
    {
        _cameraY.localRotation = Quaternion.Euler(0f, 0f, 0f);
        if (_topDown)
        {
            _topDown = false;
            _cameraX.rotation = Quaternion.Euler(_camXAngle, 0f, 0f);
        }
        else
        {
            _topDown = true;
            _cameraX.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }
}
