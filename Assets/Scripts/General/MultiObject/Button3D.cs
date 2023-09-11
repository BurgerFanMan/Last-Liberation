using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    [SerializeField] UnityEvent _onClick;
    [SerializeField] List<Collider> _colliders;

    private RayStore rayStore;
    void Start()
    {
        rayStore = FindObjectOfType<RayStore>();

        if(_colliders.Count == 0)
        {
            _colliders.Add(GetComponent<Collider>());
        }
    }

    void Update()
    {
        Collider hit = rayStore.RayHitInfo().collider;

        if (_colliders.Contains(hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                _onClick.Invoke();
            }
        }
    }
}
