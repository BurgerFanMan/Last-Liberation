using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayStore : MonoBehaviour
{

    private Vector3 hitPoint;
    private RaycastHit hit;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 40))
        {
            hitPoint = hit.point;
        }
    }

    public Vector3 RayHitPoint()
    {
        return hitPoint;
    }

    public RaycastHit RayHitInfo()
    {
        return hit;
    }
}