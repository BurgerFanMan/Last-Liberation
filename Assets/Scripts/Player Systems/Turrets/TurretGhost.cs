using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TurretGhost : MonoBehaviour
{
    [SerializeField] GameObject _meshGeneratorPrefab;
    public List<MeshRenderer> renderers;
    public List<string> ignoreColliderTags;

    private int numberOfColliders;
    private bool isInRange = true;

    void Update()
    {
        if (numberOfColliders > 0)
            return;

        if (IsInBuildZone())
        {
            if (!isInRange)
            {
                SharedVariables.buildSys.UnblockPlacement();

                isInRange = true;
            }
        }
        else if(isInRange)
        {
            SharedVariables.buildSys.BlockPlacement();

            isInRange = false;
        }
    }

    public void RenderTurretRange(Turret turret)
    {
        for(int i = 0; i < turret.subTurrets.Count; i++)
        {
            SubTurret subTurret = turret.subTurrets[i];
            GenerateSectorMesh meshGenerator = Instantiate(_meshGeneratorPrefab, transform, false).GetComponent<GenerateSectorMesh>();

            Vector3 forward = -subTurret.turretBase.right;

            meshGenerator.transform.forward = transform.right;
            meshGenerator.transform.localPosition = subTurret.turretBase.localPosition;
            meshGenerator.angle = turret.angleRange * 2f;
            meshGenerator.radius = turret.maxRange;
            meshGenerator.centerAngle = ((float)Mathf.Atan2(forward.z, forward.x) * Mathf.Rad2Deg) + 180f;

            meshGenerator.RenderSector();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ignoreColliderTags.Contains(other.tag))
            return;

        numberOfColliders += 1;

        if (numberOfColliders == 1)
            SharedVariables.buildSys.BlockPlacement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (ignoreColliderTags.Contains(other.tag))
            return;

        numberOfColliders -= 1;

        if(numberOfColliders == 0)
            SharedVariables.buildSys.UnblockPlacement();
    }

    private bool IsInBuildZone()
    {
        float distance = transform.position.sqrMagnitude;
        bool isInBuildZone = distance > SharedVariables.buildSys.minBuildRange * SharedVariables.buildSys.minBuildRange && distance < SharedVariables.buildSys.maxBuildRange * SharedVariables.buildSys.maxBuildRange;
        return isInBuildZone;
    }
}
