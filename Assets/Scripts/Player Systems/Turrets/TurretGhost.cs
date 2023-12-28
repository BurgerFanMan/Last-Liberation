using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TurretGhost : MonoBehaviour
{
    [SerializeField] GenerateSectorMesh _meshGenerator;
    public List<MeshRenderer> renderers;
    public List<string> ignoreColliderTags;

    private int numberOfColliders;
    private bool isInRange;

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

    public void RenderTurretRange(float range, float angle)
    {
        _meshGenerator.radius = range;
        _meshGenerator.angle = angle;

        _meshGenerator.RenderSector();
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
