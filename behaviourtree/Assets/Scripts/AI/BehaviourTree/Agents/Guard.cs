using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour
{
    [Header("Patrol")] [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float keepPatrolDistance = 1f;

    [Header("Weapon")] [SerializeField] private float weaponKeepDistance = 1f;
    [SerializeField] private float weaponDetectInRange = 15f;
    [SerializeField] private Transform shootingPoint;

    [Header("Player")] [SerializeField] private float playerKeepDistance = 1f;
    [SerializeField] private float playerDetectInRange = 6f;
    [SerializeField] private float playerDetectInAttackRange = 6f;
    [SerializeField] private float playerDetectOutRange = 5f;

    [HideInInspector] public Weapon Weapon = null;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(_waypoint => _waypoint.transform).ToArray();
        this.SetupBlackboard();
    }

    private void SetupBlackboard()
    {
        blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.EnemyHealth, 100);
        blackboard.SetVariable(VariableNames.TargetPatrolPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);

        Player player = FindObjectOfType<Player>();
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
    }

    private void Start()
    {
        //player in hood 
        tree = new BTRepeater(wayPoints.Length,
            new BTSelector(
                // Check if player is nearby
                new BTConditional(
                    new BTIsPlayerInHood(playerDetectInRange, transform.position),
                    // If player is nearby
                    new BTSelector(
                        // Check if guard has a weapon
                        new BTConditional(
                            new BTGuardHasWeapon(Weapon, this),
                            // If guard has a weapon, attack player
                            new BTRepeater(
                                1, // Only repeat once
                                new BTSequence(
                                    new BTIsPlayerInHood(playerDetectInAttackRange, transform.position),
                                    new BTAttackPlayer((Gun)Weapon, this, shootingPoint),
                                    new BTMoveToPlayer(agent, moveSpeed, playerDetectInAttackRange)
                                )
                            )
                        ),
                        // If guard doesn't have a weapon, search for one
                        new BTSelector(
                            new BTSequence(
                                new BTGetClosestWeaponPos(agent, weaponDetectInRange),
                                new BTMoveToWeapon(agent, moveSpeed, weaponKeepDistance)
                            )
                        )
                    )
                ),
                // If player is not nearby, patrol
                new BTSequence(
                    new BTGetNextPatrolPosition(wayPoints),
                    new BTMoveToPatrolPoint(agent, moveSpeed, keepPatrolDistance)
                )
            )
        );

        /*new BTSequence(
            // Patrol behavior
            new BTGetNextPatrolPosition(wayPoints),
            new BTMoveToPatrolPoint(agent, moveSpeed, keepPatrolDistance)
        )*/

        /*new BTSequence(
            new BTGetClosestWeaponPos(agent, weaponDetectRange),
            new BTMoveToWeapon(agent, moveSpeed, keepWeaponDistance)
            new BTMoveToPlayer(agent, moveSpeed, keepPlayerDistance)
        )*/

        /*new BTSequence(
            new BTMoveToPlayer(agent, moveSpeed, keepPlayerDistance),
            new BTAttackPlayer((Gun)Weapon, this, shootingPoint),
        )*/

        tree.SetupBlackboard(blackboard);
    }

    private void OnDisable()
    {
        //anders word valentijn boos
        EventManager.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
        TaskStatus result = tree.Tick();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.TryGetComponent(out IPickupable pickup)) return;
        pickup.Pickup();

        if (pickup is not Weapon weapon) return;
        Weapon = weapon;
    }
}