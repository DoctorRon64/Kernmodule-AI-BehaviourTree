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

    public Item item = null;

    private Transform[] wayPoints;
    private BTBaseNode tree;
    private NavMeshAgent agent;
    private Blackboard blackboard;
    private ParticleSystem particles;
    
    private const EventType guardText = EventType.GuardText;

    private bool isPickingUpWeapon = false;
    private bool isPlayerDead = false;
    private bool breakPoint = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        wayPoints = FindObjectsOfType<WayPoint>().Select(_waypoint => _waypoint.transform).ToArray();
        particles = GetComponentInChildren<ParticleSystem>();
        EventManager.AddListener<bool>(EventType.OnPlayerDied, PlayerDeadToggle);
        this.SetupBlackboard();
    }

    private void SetupBlackboard()
    {
        blackboard = new Blackboard();
        blackboard.SetVariable(VariableNames.EnemyHealth, 100);
        blackboard.SetVariable(VariableNames.TargetPatrolPosition, new Vector3(0, 0, 0));
        blackboard.SetVariable(VariableNames.CurrentPatrolIndex, -1);
        blackboard.SetVariable(VariableNames.GuardStunned, false);

        Player player = FindObjectOfType<Player>();
        if (player == null) Debug.LogError("noplayerfound");
        blackboard.SetVariable(VariableNames.TargetPlayer, player.gameObject);
    }

    private void Start()
    {
        tree = new BTRepeater(wayPoints.Length, // Repeat indefinitely
            new BTSelector(
                new BTConditional(IsGuardStunned, new BTStunNode()),
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
        if (breakPoint)
        {
            Debug.LogWarning("breakpoint");
            tree.OnReset();
            breakPoint = false;
        }

        TaskStatus result = tree.Tick();
    }

    private bool IsGuardStunned()
    {
        return blackboard.GetVariable<bool>(VariableNames.GuardStunned);
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
            new BTMoveToPatrolPoint(agent, guardText, moveSpeed, keepPatrolDistance)
        );
    }

    public void Stun()
    {
        blackboard.SetVariable(VariableNames.GuardStunned, true);
        
        StopAllCoroutines();
        StartCoroutine(StunCoroutine());
    }

    private IEnumerator StunCoroutine()
    {
        blackboard.SetVariable(VariableNames.GuardStunned, true);
        EventManager.InvokeEvent(EventType.GuardText, "Guard is Stunned!");
        particles.Play();
        
        yield return new WaitForSeconds(stunDelay);
        
        EventManager.InvokeEvent(EventType.GuardText, "Guard is free again!");
        blackboard.SetVariable(VariableNames.GuardStunned, false);
        particles.Stop();
    }
}