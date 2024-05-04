using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CustomAudioMixer : MonoBehaviour
{
    public AudioSource audioSource;
    public bool changeSpeedToPauseTime = true;

    [Range(0f, 1f)]
    public float soundMultiplier = 0.5f;
    [Tooltip("0 = None, 1 = SFX, 2 = Music, 3 = Ambience, 4 = Radio Chatter")]
    [Range(0, 4)]
    public int soundChannel;

    private float startingPitch;

    public void Start()
    {
        audioSource = audioSource == null ? GetComponent<AudioSource>() : audioSource;

        OnSoundValueChanged();
        Options.manager.onSoundValueChanged.AddListener(OnSoundValueChanged);

        startingPitch = audioSource.pitch;
    }

    void Update()
    {
        if (!changeSpeedToPauseTime)
            return;

        audioSource.pitch = startingPitch * Pause.timeScale;
    }

    void OnSoundValueChanged()
    {
        if (audioSource == null || soundChannel == 0)
            return;

        float masterVolume = Options.soundValues[0];
        float channelVolume = Options.soundValues[soundChannel];

        audioSource.volume = soundMultiplier * masterVolume * channelVolume / 4f;
    }
}
