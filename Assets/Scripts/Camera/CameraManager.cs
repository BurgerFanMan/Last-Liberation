using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("Cursor")]
    public Texture2D cursorImage;
    public Vector2 cursorOffset;

    [Header("Rotation")]
    public Transform cameraParent;
    [Range(0f, 100f)]
    public float orbitSpeedY;

    [Header("Angle")]
    public bool topDown;
    public float cameraXAngle;

    [Header("Movement")]
    public float speed;
    public float targetPosWeight = 1f;
    public CameraMode cameraMode = CameraMode.SmoothDamp;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        if(cursorImage != null)
            Cursor.SetCursor(cursorImage, cursorOffset, CursorMode.Auto);

        cameraParent.localRotation = Quaternion.Euler(0f, 0f, 0f);

        ChangeMode();
    }

    // Update is called once per frame
    void Update()
    {
        if (Pause.isPaused)
            return;

        if(Input.GetKey(InputManager.GetValue("camera_rotate")))
        {
            float cameraYAngle = Input.GetAxis("Mouse X") * Time.deltaTime * orbitSpeedY;
            cameraYAngle += cameraParent.eulerAngles.y;

            cameraParent.rotation = Quaternion.Euler(cameraParent.eulerAngles.x, cameraYAngle, 0f);
        }

        if (Input.GetKeyDown(InputManager.GetValue("camera_changemode")))
            ChangeMode();

        //handling movement
        if (Input.GetKey(InputManager.GetValue("camera_lockposition")))
            return;

        Vector3 targetPos = RayStore.GroundedHitPoint;
        targetPos.y = 0f;
        Vector3 newPos = (Vector3.zero + (targetPos * targetPosWeight)) / (1f + targetPosWeight);

        Vector3 movement;

        if (cameraMode == CameraMode.Lerp)
            movement = Vector3.Lerp(cameraParent.position, newPos, speed);
        else if (cameraMode == CameraMode.SmoothDamp)
            movement = Vector3.SmoothDamp(cameraParent.position, newPos, ref velocity, 1f / speed);
        else if (cameraMode == CameraMode.MoveTowards)
            movement = Vector3.MoveTowards(cameraParent.position, newPos, speed);
        else
            movement = newPos;

        cameraParent.position = movement;
    }

    public void ChangeMode()
    {
        cameraParent.localRotation = Quaternion.Euler(0f, 0f, 0f);

        if (topDown)
            cameraParent.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
            cameraParent.rotation = Quaternion.Euler(cameraXAngle, 0f, 0f);

        topDown = !topDown;
    }
}

public enum CameraMode
{
    Lerp,
    SmoothDamp,
    MoveTowards,
    NoInterpolation
}
