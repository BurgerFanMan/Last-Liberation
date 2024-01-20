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

        if (Physics.Raycast(ray, out RayStore.hitInfo, 40f))
        {
            RayStore.hitPoint = RayStore.hitInfo.point;
        }

	if (Physics.Raycast(ray, out RayStore.groundedHitInfo, 40f, 1<<7))
        {
            RayStore.GroundedHitPoint = RayStore.groundedHitInfo.point;
        }
    }
}

public static class RayStore
{
    public static RaycastHit hitInfo;
    public static Vector3 hitPoint;
    public static RaycastHit groundedHitInfo;
    public static Vector3 GroundedHitPoint;
}