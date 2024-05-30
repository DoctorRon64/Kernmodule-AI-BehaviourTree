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

    public Weapon Weapon = null;

    public delegate bool ConditionDelegate();

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private bool isPickingUpWeapon = false;
    private bool isPlayerDead = false;
    private bool breakPoint = false;

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
        EventManager.AddListener<bool>(EventType.OnPlayerDied, PlayerDeadToggle);
    }

    private void Start()
    {
        tree = new BTRepeater(-1, // Repeat indefinitely
            new BTSelector(
                new BTConditional(() => isPickingUpWeapon,
                    new BTSequence(
                        new BTGetClosestWeaponPos(agent, weaponDetectInRange),
                        new BTMoveToWeapon(agent, moveSpeed, weaponKeepDistance),
                        new BTAction(() =>
                        {
                            isPickingUpWeapon = false;
                            return TaskStatus.Success;
                        })
                    )
                ),
                new BTConditional(() => !IsPlayerNearby() && !HasWeapon(),
                    new BTSequence(
                        new BTGetNextPatrolPosition(wayPoints),
                        new BTMoveToPatrolPoint(agent, moveSpeed, keepPatrolDistance)
                    )
                ),
                new BTConditional(HasWeapon,
                    new BTSelector(
                        new BTConditional(IsPlayerNearby,
                            new BTRepeater(-1,
                                new BTSequence(
                                    new BTAttackPlayer((Gun)Weapon, this, shootingPoint),
                                    new BTMoveToPlayer(agent, moveSpeed, playerDetectInAttackRange)
                                )
                            )
                        ),
                        new BTConditional(() => !isPlayerDead && !IsPlayerNearby(),
                            new BTSequence(
                                new BTGetNextPatrolPosition(wayPoints),
                                new BTMoveToPatrolPoint(agent, moveSpeed, keepPatrolDistance)
                            )
                        )
                    )
                ),
                new BTConditional(() => !HasWeapon() && !isPickingUpWeapon,
                    new BTSequence(
                        new BTAction(() =>
                        {
                            isPickingUpWeapon = true;
                            return TaskStatus.Success;
                        }),
                        new BTGetClosestWeaponPos(agent, weaponDetectInRange),
                        new BTMoveToWeapon(agent, moveSpeed, weaponKeepDistance)
                    )
                )
            )
        );

        tree.SetupBlackboard(blackboard);


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
    }

    private void OnDisable()
    {
        //anders word valentijn boos
        EventManager.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
        if (breakPoint)
        {
            tree.OnReset();
            breakPoint = false;
        }

        TaskStatus result = tree.Tick();
    }

    private void PlayerDeadToggle(bool _toggle)
    {
        isPlayerDead = _toggle;
        breakPoint = true;
        Debug.Log("IsPlayerActive SET to: " + _toggle);
    }

    private bool IsPlayerNearby()
    {
        GameObject player = blackboard.GetVariable<GameObject>(VariableNames.TargetPlayer);
        if (player == null) { Debug.Log("player not nearby"); return false;}

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= playerDetectInRange;
    }
    
    private bool HasWeapon()
    {
        return Weapon != null;
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.TryGetComponent(out IPickupable pickup)) return;
        pickup.Pickup();

        if (pickup is not Weapon weapon) return;
        Weapon = weapon;
    }
}