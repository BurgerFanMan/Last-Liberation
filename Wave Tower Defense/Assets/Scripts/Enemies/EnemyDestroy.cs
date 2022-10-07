using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroy : MonoBehaviour
{
    [SerializeField] float _force;

    public float _descendRate = 0.01f;
    private void Awake()
    {
        foreach(Rigidbody cube in transform.GetComponentsInChildren<Rigidbody>())
        {
            cube.AddForce(transform.forward * Random.Range(-_force / 2, -_force * 2));
        }

        Invoke("RemoveRigidbodies", 5f);
    }

    private void Update()
    {
        transform.position -= new Vector3(0f, _descendRate * Time.deltaTime, 0f);

        if(transform.position.y < -1f)
        {
            Destroy(gameObject);
        }
    }

    void RemoveRigidbodies()
    {
        for(int i = transform.childCount; i > 0; i--)
        {
            Destroy(transform.GetChild(i - 1).GetComponent<Rigidbody>());
        }
    }
}
