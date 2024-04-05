using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IDamageable
{
    public float moveSpeed = 3;
    public float keepPatrolDistance = 1f;
    public float weaponDetectDistance = 15f;
    public float playerDetectDistance = 5f;
    public int MaxHealth { get; } = 100;
    public int Health { get; set; }

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Animator animator;

    private void Awake()
    {
        Health = MaxHealth;
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
        blackboard.SetVariable(VariableNames.WeoponsInScene, FindObjectsOfType<Weapon>());

        tree = new BTRepeater(wayPoints.Length,
            new BTSelector(
                new BTSequence(
                    new BTIsPlayerInHood(playerDetectDistance, transform.position),
                    new BTMoveToClosestWeapon(agent, moveSpeed, weaponDetectDistance)
                ),
                new BTSequence(
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetPosition, keepPatrolDistance)
                )
            )
        );

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
        Debug.Log(result);
    }

    public void TakeDamage(int _damage)
    {
        Health -= _damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
    }
}