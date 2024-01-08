using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlide : MonoBehaviour
{
    public bool isSlidingOpen = false;
    public bool adjustScale = true; //rounds the scale when close to the target
    public float speed = 2f;
    public Vector3 targetOpenScale;
    public Vector3 targetCloseScale;

    private void Update()
    {
        Vector3 targetScale = isSlidingOpen ? targetOpenScale : targetCloseScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, speed * Time.deltaTime);

        if (!adjustScale)
            return;

        if ((transform.localScale - targetScale).magnitude < 0.05f)
            transform.localScale = targetScale;
    }

    public void SlideUI()
    {
        isSlidingOpen = !isSlidingOpen;
    }
    public void SlideUI(bool slideOpen)
    {
        isSlidingOpen = slideOpen;
    }
}
