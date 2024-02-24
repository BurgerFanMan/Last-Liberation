using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TurretGhost : MonoBehaviour
{
    [SerializeField] GameObject _meshGeneratorPrefab;
    public List<MeshRenderer> renderers;
    public List<string> ignoreColliderTags = new List<string>{ "Enemy" };

    private int numberOfColliders;
    private bool isInRange = true;

    private BuildSystem buildSys;

    void Reset()
    {
        renderers = GetComponentsInChildren<MeshRenderer>().ToList();
    }

    private void Start()
    {
        buildSys = SharedVariables.buildSys;

        SharedVariables.buildSys.UnblockPlacement();
    }

    void Update()
    {
        float distanceSqr = transform.position.sqrMagnitude;

        if (distanceSqr < buildSys.minBuildRange * buildSys.minBuildRange)
            transform.position = transform.position.normalized * buildSys.minBuildRange;
        else if (distanceSqr > buildSys.maxBuildRange * buildSys.maxBuildRange)
            transform.position = transform.position.normalized * buildSys.maxBuildRange;
    }

    public void RenderTurretRange(Turret turret)
    {
        for(int i = 0; i < turret.subTurrets.Count; i++)
        {
            SubTurret subTurret = turret.subTurrets[i];
            GenerateSectorMesh meshGenerator = Instantiate(_meshGeneratorPrefab, transform, false).GetComponent<GenerateSectorMesh>();

            meshGenerator.transform.forward = transform.right;
            meshGenerator.transform.localPosition = subTurret.turretBase.localPosition;
            meshGenerator.angle = turret.angleRange * 2f;
            meshGenerator.radius = turret.maxRange;
            meshGenerator.centerAngle = subTurret.turretBase.eulerAngles.y + 180f;
            
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

        if(numberOfColliders == 0 && isInRange)
            SharedVariables.buildSys.UnblockPlacement();
    }
}
