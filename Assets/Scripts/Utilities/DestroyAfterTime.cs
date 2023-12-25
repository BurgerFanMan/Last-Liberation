using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] float delay = 5f;

    private void Start()
    {
        Invoke("DestroyThis", delay);
    }

    void DestroyThis()
    {
        Destroy(gameObject);
    }
}
