using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IStunnable
{
    [Header("Patrol")] 
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float keepPatrolDistance = 1f;

    [Header("Weapon")]
    [SerializeField] private float weaponKeepDistance = 1f;
    [SerializeField] private float weaponDetectInRange = 15f;
    [SerializeField] private Transform shootingPoint;

    [Header("Player")]
    [SerializeField] private float playerKeepDistance = 1f;
    [SerializeField] private float playerKeepAttackDistance = 2f;
    [SerializeField] private float playerDetectInRange = 5f;
    
    [Header("Stun")]
    [SerializeField] private float stunDelay = 5f;
    private Coroutine resetStunCoroutine;
    
    public Item item = null;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;

    private EventType guardText;

    private bool isPickingUpWeapon = false;
    private bool isPlayerDead = false;
    private bool breakPoint = false;
    private bool isGuardStunned = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(_waypoint => _waypoint.transform).ToArray();
        guardText = EventType.GuardText;
        EventManager.AddListener<bool>(EventType.OnPlayerDied, PlayerDeadToggle);
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
        tree = new BTRepeater(wayPoints.Length, // Repeat indefinitely
            new BTSelector(
                new BTConditional(() => isPickingUpWeapon,
                    new BTSequence(
                        new BTGetClosestWeaponPos(agent, weaponDetectInRange),
                        new BTMoveToWeapon(agent, guardText, moveSpeed, weaponKeepDistance),
                        new BTMoveToPlayer(agent, guardText, moveSpeed, playerKeepDistance),
                        new BTAction(() =>
                        {
                            isPickingUpWeapon = false;
                            return TaskStatus.Success;
                        })
                    )
                ),
                new BTConditional(() => isPlayerDead, CreatePatrolSequence()),
                new BTConditional(() => !IsPlayerNearby() && !HasWeapon(), CreatePatrolSequence()),
                new BTConditional(HasWeapon,
                    new BTSelector(
                        new BTConditional(IsPlayerNearby,
                            new BTSequence(
                                new BTMoveToPlayer(agent, guardText, moveSpeed, playerKeepAttackDistance),
                                new BTAttackPlayer((Gun)item, this, shootingPoint)
                            )
                        ),
                        new BTConditional(() => !isPlayerDead && !IsPlayerNearby(), CreatePatrolSequence())
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
                        new BTMoveToWeapon(agent, guardText, moveSpeed, weaponKeepDistance)
                    )
                )
            )
        );

        tree.SetupBlackboard(blackboard);
    }

    private void OnDisable()
    {
        //anders word valentijn boos
        EventManager.RemoveAllListeners();
    }

    private void FixedUpdate()
    {
        if (isGuardStunned)
        {
            return;
        }
        
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
        if (player == null)
        {
            Debug.Log("player not nearby");
            return false;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        return distanceToPlayer <= playerDetectInRange;
    }

    private bool HasWeapon()
    {
        return item != null;
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (!_other.TryGetComponent(out IPickupable pickup)) return;
        pickup.Pickup();

        if (pickup is not Item weapon) return;
        item = weapon;
    }

    private BTSequence CreatePatrolSequence()
    {
        return new BTSequence(
            new BTGetNextPatrolPosition(wayPoints),
            new BTMoveToPatrolPoint(agent, guardText, moveSpeed, keepPatrolDistance),
            new BTAction(() =>
            {
                EventManager.InvokeEvent(EventType.OnPlayerAttack, false);
                return TaskStatus.Success;
            })
        );
    }

    public void Stun()
    {
        Debug.Log("Character stunned!");
        isGuardStunned = true;
        EventManager.InvokeEvent(EventType.OnPlayerAttack, false);
        
        if (resetStunCoroutine != null) StopCoroutine(resetStunCoroutine);
        resetStunCoroutine = StartCoroutine(ResetStun());
    }

    private IEnumerator ResetStun()
    {
        yield return new WaitForSeconds(stunDelay);
        
        Debug.Log("Character recovered from stun!");
        isGuardStunned = false;
    }
}