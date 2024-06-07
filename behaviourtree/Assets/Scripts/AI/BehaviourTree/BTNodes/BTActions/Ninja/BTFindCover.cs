using UnityEngine;

public class BTFindCover : BTBaseNode
{
    private readonly Transform[] coverPoints;
    private readonly Transform agentTransform;

    public BTFindCover(Transform[] _coverPoints, Transform _agentTransform)
    {
        coverPoints = _coverPoints;
        agentTransform = _agentTransform;
    }

    protected override TaskStatus OnUpdate()
    {
        Transform nearestCover = null;
        float minDistance = float.MaxValue;

        foreach (var cover in coverPoints)
        {
            float distance = Vector3.Distance(agentTransform.position, cover.position);
            if (!(distance < minDistance)) continue;
            minDistance = distance;
            nearestCover = cover;
        }

        if (nearestCover == null) return TaskStatus.Failed;
        blackboard.SetVariable(VariableNames.CoverPos, nearestCover.position);
        return TaskStatus.Success;
    }
}