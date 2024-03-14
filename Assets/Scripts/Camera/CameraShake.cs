using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float intensityModifier = 1f;
    public float currentShakeIntensity;
    public float currentDuration;
    public int currentPriority;

    private Vector3 originalPosition;

    private void Awake()
    {
        SharedVariables.cameraShake = this;
    }

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentDuration < 0f || Pause.isPaused)
            return;

        transform.localPosition = originalPosition + (currentShakeIntensity * Pause.timeScale * Random.insideUnitSphere / 100f);

        currentDuration -= Pause.adjTimeScale;

        if (currentDuration < 0f)
            transform.localPosition = originalPosition;
    }

    public void ShakeCamera(float intensity, float duration, int priority) //higher priority overrides lower priority
    {
        if (priority <= currentPriority && currentDuration > 0f)
            return;

        currentShakeIntensity = intensity;
        currentDuration = duration;
        currentPriority = priority;
    }
}
