using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float delay = 5f;
    [SerializeField] bool usePauseTime = true;

    private float timeDone;

    void Update()
    {
        if (timeDone < delay)
        {
            timeDone += usePauseTime ? Pause.adjTimeScale : Time.deltaTime;

            return;
        }

        Destroy(gameObject);
    }
}
