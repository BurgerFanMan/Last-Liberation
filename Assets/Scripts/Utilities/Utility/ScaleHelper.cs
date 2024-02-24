using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleHelper : MonoBehaviour
{
    public bool isSlidingOpen = false;
    public bool adjustScale = true; //rounds the scale when close to the target
    public bool isFullyScaledOnStart = false;
    public float speed = 2f;
    public Vector3 targetOpenScale;
    public Vector3 targetCloseScale;

    private bool destroyOnClose;

    private void Start()
    {
        if (isFullyScaledOnStart)
            transform.localScale = isSlidingOpen ? targetOpenScale : targetCloseScale;
    }
    private void Update()
    {
        Vector3 targetScale = isSlidingOpen ? targetOpenScale : targetCloseScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);

        if (!adjustScale)
            return;

        if ((transform.localScale - targetScale).sqrMagnitude < 0.000001f)
        {
            if (targetScale == targetCloseScale && destroyOnClose)
                Destroy(gameObject, 0f);
            transform.localScale = targetScale;
        }
    }

    public void Scale()
    {
        isSlidingOpen = !isSlidingOpen;
    }
    public void Scale(bool slideOpen)
    {
        isSlidingOpen = slideOpen;
    }
    
    public void ScaleClosedAndDestroy()
    {
        Scale(false);

        destroyOnClose = true;
    }
}
