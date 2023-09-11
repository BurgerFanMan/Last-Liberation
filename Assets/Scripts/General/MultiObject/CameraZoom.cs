using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] float _zoomSpeed;
    [SerializeField] float _turnSpeed;
    [SerializeField] List<Vector3> _defaultRotation;
    [SerializeField] List<Vector3> _defaultPosition;

    private Vector3 zoomToPos;
    private Vector3 turnToRot;
    private bool zooming;
    private Transform cameraTrans;

    private void Start()
    {
        cameraTrans = Camera.main.transform;
    }

    void Update()
    {
        if (zooming)
        {
            cameraTrans.position = Vector3.Lerp(cameraTrans.position, zoomToPos, 
                _zoomSpeed * Time.deltaTime);

            cameraTrans.rotation = Quaternion.Lerp(cameraTrans.rotation, 
                Quaternion.Euler(turnToRot), _turnSpeed * Time.deltaTime);
        }
    }

    public void ZoomStart(Vector3 pos, Vector3 rot)
    {
        zooming = true;
        zoomToPos = pos;
        turnToRot = rot;
    }

    public void ZoomStart(Vector3 pos)
    {
        zooming = true;
        zoomToPos = pos;
        turnToRot = _defaultRotation[0];
    }

    public void ZoomStart(int index)
    {
        zooming = true;
        zoomToPos = _defaultPosition[index];
        turnToRot = _defaultRotation[index];
    }
}
