using UnityEngine;

class ParabolicBullet : Bullet
{
    [Header("Arc Settings")]
    [Range(0f, 2f)]
    public float heightFactor = 0.4f;


    private float aCoefficient;
    private float distanceToTarget;

    private float distanceTravelled;

    private Vector3 directionToTarget;

    public override void OnSpawn(Vector3 position, Quaternion rotation, Vector3 targetPosition)
    {
        base.OnSpawn(position, rotation, targetPosition);

        Vector3 adjTargetPos = targetPoint;
        adjTargetPos.y = 0f;
        Vector3 adjStartPos = startPoint;
        adjStartPos.y = 0f;

        distanceToTarget = Vector3.Distance(adjStartPos, adjTargetPos);
        aCoefficient = heightFactor / distanceToTarget;
        directionToTarget = (adjTargetPos - adjStartPos).normalized;

        distanceTravelled = 0f;
    }

    protected override void Move()
    {
        if (Pause.isPaused)
            return;

        distanceTravelled += _speed * Pause.adjTimeScale;

        float cCoefficient = startPoint.y - (startPoint.y * distanceTravelled / distanceToTarget);
        float height = (-aCoefficient * distanceTravelled * distanceTravelled) + (heightFactor * distanceTravelled) + cCoefficient;
        
        Vector3 moveVector = _speed * directionToTarget * Pause.adjTimeScale;
        moveVector.y = height - transform.position.y;

        transform.position += moveVector;
        transform.rotation = Quaternion.LookRotation(moveVector.normalized);
    }
}