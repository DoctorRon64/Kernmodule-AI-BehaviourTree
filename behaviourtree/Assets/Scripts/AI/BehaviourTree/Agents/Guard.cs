using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    public float moveSpeed = 3;
    public float keepDistance = 1f;
    public float playerDetectDistance = 3f;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(waypoint => waypoint.transform).ToArray();
    }

    private void Start()
    {
        Blackboard blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.EnemyHealth, 100);
        blackboard.SetVariable(VariableNames.TargetPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);
        blackboard.SetVariable(VariableNames.PlayerTransform, FindObjectOfType<Player>().transform);

        tree =
            new BTRepeater(wayPoints.Length,
                new BTSequence(
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetPosition, keepDistance),
                    new BTPlayerInHood(playerDetectDistance, transform.position)
                )
            );

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
        Debug.Log(result);
    }
}