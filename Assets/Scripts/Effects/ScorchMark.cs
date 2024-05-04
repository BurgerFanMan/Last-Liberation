using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScorchMark : MonoBehaviour
{
    public float fadeStartTime = 10f;
    public float fadeDuration = 10f;

    public bool usePauseTime = true;

    public SpriteRenderer spriteRenderer;


    private float timeSinceStart;

    private void Start()
    {
        float xRotation = 0f;
        float yRotation = Random.Range(0f, 360f);
        float zRotation = 0f;

        transform.Rotate(new Vector3(xRotation, yRotation, zRotation));

        spriteRenderer = spriteRenderer == null ? GetComponent<SpriteRenderer>() : spriteRenderer;
        spriteRenderer = spriteRenderer == null ? GetComponentInChildren<SpriteRenderer>() : spriteRenderer;
    }

    private void Update()
    {
        timeSinceStart += usePauseTime ? Pause.adjTimeScale : Time.deltaTime;

        if (timeSinceStart < fadeStartTime)
            return;

        float timeSinceFadeStart = timeSinceStart - fadeStartTime;

        Color color = spriteRenderer.color;
        color.a = 1f - ( timeSinceFadeStart / fadeDuration );
        spriteRenderer.color = color;

        if (timeSinceStart < fadeStartTime + fadeDuration)
            return;

        Destroy(gameObject);
    }
}