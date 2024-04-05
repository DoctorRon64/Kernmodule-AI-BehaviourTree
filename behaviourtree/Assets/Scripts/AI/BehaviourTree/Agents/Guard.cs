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
        blackboard.SetVariable(VariableNames.TargetPatrolPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);
        blackboard.SetVariable(VariableNames.PlayerTransform, FindObjectOfType<Player>().transform);

        Debug.Log(blackboard.GetVariable<Transform>(VariableNames.PlayerTransform));
        
        tree = new BTRepeater(wayPoints.Length,
            new BTSelector(
                new BTConditional(
                    new BTIsPlayerInHood(playerDetectDistance, transform.position),
                    new BTSequence(
                        new BTGetClosestWeaponPos(agent, weaponDetectDistance),
                        new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetWeaponPosition ,weaponDetectDistance),
                        new BTMoveToPlayer(agent, moveSpeed , 1f)
                        //new BTAttackPlayer(agent, blackboard.GetVariable<Transform>(VariableNames.PlayerTransform))
                    )
                ),
                new BTSequence(
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetPatrolPosition, keepPatrolDistance)
                )
            )
        );
        
        /*tree = new BTRepeater(
            wayPoints.Length,
            new BTSelector(
                new BTSequence(
                    new BTIsPlayerInHood(playerDetectDistance, transform.position),
                    new BTGetClosestWeaponPos(agent, weaponDetectDistance),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetPosition, weaponDetectDistance)
                ),
                new BTSequence(
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPosition(agent, moveSpeed, VariableNames.TargetPosition, keepPatrolDistance)
                )
            )
        );*/

        tree.SetupBlackboard(blackboard);
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
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