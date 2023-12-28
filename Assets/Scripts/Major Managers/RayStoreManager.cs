using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayStoreManager : MonoBehaviour
{
    private void Start()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RayStore.hitInfo, 40))
        {
            RayStore.hitPoint = RayStore.hitInfo.point;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RayStore.hitInfo, 40))
        {
            RayStore.hitPoint = RayStore.hitInfo.point;
        }
    }
}

public static class RayStore
{
    public static RaycastHit hitInfo;
    public static Vector3 hitPoint;
    public static Vector3 GroundedHitPoint { get { return new Vector3(hitPoint.x, 0f, hitPoint.z); } }
}