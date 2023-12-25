using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] List<TurretInfo> turretInfos;

    private TurretInfo selectedTurret;
    private GameObject ghost;

    private void Update()
    {
        if (ghost != null)
            ghost.transform.position = RayStore.hitPoint;
    }

    public void SelectTurret(int turretInfoIndex)
    {
        selectedTurret = turretInfos[turretInfoIndex];
        ghost = selectedTurret.turretGhost;
    }

    public void DeselectTurret()
    {
        selectedTurret = null;
        ghost = null;

        //unslow time
    }

    public void PlaceTurret()
    {

    }

    public bool IsBuildAllowed(Vector3 position)
    {   //instead of this slap a trigger collider on the ghost with a tag checker
        return true;
    }
}
