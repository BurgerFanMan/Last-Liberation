using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSoundSpeed : MonoBehaviour
{
    public AudioSource audioSource;
    public bool changeSpeedToPauseTime = true;

    private float startingPitch;

    void Start()
    {
        audioSource = audioSource == null ? GetComponent<AudioSource>() : audioSource;

        startingPitch = audioSource.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        if (!changeSpeedToPauseTime)
            return;

        audioSource.pitch = startingPitch * Pause.timeScale;
    }
}
