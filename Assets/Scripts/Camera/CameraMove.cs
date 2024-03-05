using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float speed;
    public float targetPosWeight = 1f;
    public CameraMode cameraMode = CameraMode.SmoothDamp;
    public Transform cameraParent;

    private Vector3 velocity = Vector3.zero;
    private Vector3 origin = new Vector3(0f, 0f, 0f);

    void Update()
    {
        if (!Pause.isPaused && !Input.GetKey(InputManager.GetValue("camera_lockposition")))
        {
            Vector3 targetPos;
            targetPos = RayStore.GroundedHitPoint;
            targetPos.y = 0;
            Vector3 newPos = ((origin + (targetPos * targetPosWeight)) / (1f + targetPosWeight));

            Vector3 movement;
            
            if(cameraMode == CameraMode.Lerp)
                movement = Vector3.Lerp(transform.position, newPos, speed);
            else if(cameraMode == CameraMode.SmoothDamp)
                movement = Vector3.SmoothDamp(transform.position, newPos, ref velocity, 1f/speed);
            else if(cameraMode == CameraMode.MoveTowards)
                movement = Vector3.MoveTowards(transform.position, newPos, speed);
            else
                movement = newPos;
            
            cameraParent.position = movement;
        }
    }
}

public enum CameraMode{
    Lerp,
    SmoothDamp,
    MoveTowards,
    NoInterpolation
}
