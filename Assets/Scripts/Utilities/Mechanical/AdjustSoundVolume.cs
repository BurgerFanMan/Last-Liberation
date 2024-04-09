using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustSoundVolume : MonoBehaviour
{
    public AudioSource audioSource;
    [Range(0f, 1f)]
    public float soundMultiplier = 0.5f;
    [Tooltip("1 = SFX, 2 = Music, 3 = Ambience, 4 = Radio Chatter")]
    [Range(1, 4)]
    public int soundChannel;

    void Start()
    {
        audioSource = audioSource == null ? GetComponent<AudioSource>() : audioSource;

        OnSoundValueChanged();

        Options.manager.onSoundValueChanged.AddListener(OnSoundValueChanged);
    }

    void OnSoundValueChanged()
    {
        if (audioSource == null)
            return;

        float masterVolume = Options.soundValues[0];
        float channelVolume = Options.soundValues[soundChannel];

        audioSource.volume = soundMultiplier * masterVolume * channelVolume / 4f;
    }
}
