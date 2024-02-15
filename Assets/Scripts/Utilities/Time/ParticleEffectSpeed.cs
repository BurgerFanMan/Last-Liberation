using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEffectSpeed : MonoBehaviour
{
    public ParticleSystem particleEffect;
    public bool changeSpeedToPauseTime = true;

    void Start()
    {
        particleEffect = particleEffect == null ? GetComponent<ParticleSystem>() : particleEffect;
    }

    // Update is called once per frame
    void Update()
    {
        if (!changeSpeedToPauseTime)
            return;

        var main = particleEffect.main;
        
        main.simulationSpeed = Pause.timeScale;
    }
}
